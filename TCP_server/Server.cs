using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Csharp_UnitTest;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace TCP_server
{
    public class Server
    {
        public static string JsonFileName = "FootballPlayers.json";
        public static List<FootballPlayer> footballPlayers = new List<FootballPlayer>();
        //public static List<FootballPlayer> footballPlayers = new List<FootballPlayer>() 
        //{
        //    new FootballPlayer("ronaldinio", 45100, 54), new FootballPlayer("mezi", 5800, 42), new FootballPlayer("ronmez", 38100, 97)
        //};
        public void Start()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 2121);

            tcpListener.Start();

            //FootballPlayer footballPlayer = new FootballPlayer("ron weaslyey",1,1);
            //footballPlayers.Add(footballPlayer);

            //WriteToJson(footballPlayers, JsonFileName);

            footballPlayers = ReadJson(JsonFileName);

            while (true)
            {
                Task.Run(() => { DoServerSTuff(tcpListener.AcceptTcpClient()); });
            }
        }

        public void DoServerSTuff(TcpClient socket)
        {
            using (StreamWriter sw = new StreamWriter(socket.GetStream()))
            using (StreamReader sr = new StreamReader(socket.GetStream()))
            {
                WriteToConsoleStreamWriter(sw, "Hi welcome to the server, here you can write commands like, 'HentAlle', 'Hent x' hvor x er et tal/id, 'Gem'");

                string line = sr.ReadLine();
                WriteToConsoleStreamWriter(sw, line);  // could make another while loop so that one client can keep the connection and keep typing commands

                string Func = line.Split(" ")[0];

                switch (Func.ToLower())
                {
                    case "hent":
                        FootballPlayer footballPlayer = null;
                        try
                        {
                            footballPlayer = Hent(int.Parse(line.Split(" ")[1]));
                        }
                        catch (Exception)
                        {
                            WriteToConsoleStreamWriter(sw, "Something whent wrong, couldn't find a football player with the chosen id");
                        }
                        finally
                        {
                            WriteToConsoleStreamWriter(sw, footballPlayer?.ToString());
                        }
                        break;
                    case "hentalle":
                        List<FootballPlayer> footballPlayers = HentAlle();
                        foreach (var item in footballPlayers)
                        {
                            WriteToConsoleStreamWriter(sw, item.ToString());
                        }
                        break;
                    case "gem":
                        Gem(line);
                        break;
                    default:
                        break;
                }
            }
        }

        public void WriteToConsoleStreamWriter(StreamWriter sw, string line = "")
        {
            sw.WriteLine(line);
            sw.Flush();

            Console.WriteLine(line);
        }

        public FootballPlayer Hent(int id)
        {
            FootballPlayer footballPlayer = ReadJson(JsonFileName).Find(x => x.Id == id);
            if (footballPlayer == null) throw new NullReferenceException();
            else return footballPlayer;          
        }

        public List<FootballPlayer> HentAlle()
        {
            return ReadJson(JsonFileName);
        }

        public void Gem(string footballPlayerProperties) // fx: Gem ronaldinio 50.5 25 
        {
            string[] stats = footballPlayerProperties.Split(" "); // det første mellemrum vil være efter gem så den værdi springer vi over og startet med index 1

            FootballPlayer footballPlayer = new FootballPlayer(stats[1], double.Parse(stats[2]), int.Parse(stats[3]));

            footballPlayers = ReadJson(JsonFileName);
            footballPlayers.Add(footballPlayer);

            WriteToJson(footballPlayers, JsonFileName);
        }


        public static List<FootballPlayer> ReadJson(string JsonFileName)
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<List<FootballPlayer>>(jsonFileReader.ReadToEnd());
            }
        }
        public static void WriteToJson(List<FootballPlayer> footballPlayers, string JsonFileName)
        {
            using (FileStream outputSource = File.OpenWrite(JsonFileName))
            {
                var writter = new Utf8JsonWriter(outputSource, new JsonWriterOptions
                {
                    SkipValidation = false,
                    Indented = true
                });
                JsonSerializer.Serialize(writter, footballPlayers.ToArray());
            }
        }
    }
}