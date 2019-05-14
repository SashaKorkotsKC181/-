using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace troleyborus
{

    class Program
    {
        const string rout = "rout.txt";
        const string name_graphics = "graphics.txt";
        const string file_answer = "output.html";
        static bool station_in_rout(List<string[]> summary_table, char direction, string user_rout, int station, List<int> stations_in_rout)
        {
            List<int> list = Station_rout(summary_table,direction,user_rout);
            stations_in_rout.Clear();
            foreach (int i in list)
            {
                if (i == station)
                {
                    return true;
                }
            }

            return false; 
        }
        static void Output(List<string[]> summary_table,char direction, List<int> stations, string user_rout,StreamWriter write, bool b)
        {                        
            for (int i = 0; i < stations.Count; i++)
            {
                write.WriteLine("<td><table border=1>");
                write.WriteLine("<tr>");                
                write.Write("<td>" + summary_table[stations[i]][0] + "</td>");
                if (b)
                    write.WriteLine("<td>" + direction + user_rout + "</td>");
                for (int j = 1; j < summary_table[stations[i]].Count() - 1; j++)
                {
                    if (user_rout == summary_table[0][j].Remove(0, 1) && summary_table[0][j].Contains(direction))
                        {                            
                            write.WriteLine("<tr>");
                            if (summary_table[stations[i]][j] != "" )
                            {
                                write.WriteLine("<td>" + summary_table[stations[i]][j] + "</td>");
                            }
                            else if (!b)
                            {
                                write.WriteLine("<td bgcolor=" + "#ffcc00" + ">" + "_____" + "</td>");
                            } 
                            write.WriteLine("</tr>");                            
                        }                                            
                } 
                write.WriteLine("</table></td>");
            }            
        }
        static string Сhoice_rout()
        {
            string[] st = File.ReadAllText(rout,System.Text.Encoding.Default).Split(' ');
            int num = 1;
            bool b = false;
            do
            {
                if (b)
                    Console.WriteLine("Помилка введення, повторіть спробу");
                num = 1;
                foreach (string k in st)
                {
                    if (k != "всі")       
                        Console.WriteLine("{0} Маршрут({1})",num,k);
                    else Console.WriteLine("{0} Обрати всі маршрути", num);
                    num++;
                }                            
                try
                {
                    num = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    num = -1;
                }
                b = true;
            } while (num < 1 || num > st.Count());
            return st[num - 1];
        }
        static char Choice_direction(string user_rout)        
        {
            bool b = false;
            string[] station_plus = File.ReadAllText("+" + user_rout + ".txt", System.Text.Encoding.Default).Split('\t');
            string[] station_minus = File.ReadAllText("-" + user_rout + ".txt", System.Text.Encoding.Default).Split('\t');
            char answer = ' ';
            string index = "";
            do
            {
                if (b)
                    Console.WriteLine("Помилка введення, повторіть спробу");
                Console.WriteLine("Маршрут ({0})\n1 З {1} на {2}\n2 З {3} на {4}",
                                   user_rout, station_plus[0], station_plus[station_plus.Count() - 2],
                                   station_minus[0], station_minus[station_minus.Count() - 2]);
                index = Console.ReadLine();
                if (index == "2") answer = '-';
                else if (index == "1") answer = '+';
                else answer = '@';
                b = true;
                //захист від введення числа яке не відповідає списку
            } while (answer == '@');
            return answer;
        }
        static void Input(List<string[]> summary_table)            
        {            
            StreamReader read = new StreamReader(name_graphics, System.Text.Encoding.Default);
            do
            {
                summary_table.Add(read.ReadLine().Split('*')[0].Split('\t'));
            } while (summary_table[summary_table.Count - 1][0] != "@");
            read.Close();
        }
        static int Contains(List<string[]> summary_table, string user_word, List<int> station_of_rout)
        {
            List<int> contains_find = new List<int>();
            Regex reg = new Regex(user_word + ".*", RegexOptions.IgnoreCase);                        
            int index = 1;
            for (int i = 0; i < station_of_rout.Count; i++)
            {                
                if (reg.IsMatch(summary_table[station_of_rout[i]][0]))
                {
                    Console.WriteLine(index + " " + summary_table[station_of_rout[i]][0]);
                    contains_find.Add(station_of_rout[i]);
                    index++;
                }
            }
            if (contains_find.Count != 0)
            {
                Console.WriteLine("Щоб обрати станцію введіть її номер із списку");
                int x = Convert.ToInt32(Console.ReadLine());
                return contains_find[x - 1];
            }
            else
            {
                Console.WriteLine("немає станцій з подібною назвою, перевірте правельність введеного\nВведіть назву зупинки");
                return -1;
            }
        }
        static List<int> Search_Stains(List<string[]> summary_table, List<int> station_rout, string user_roat) 
        {
            int t = 0;
            List<int> st = new List<int>();
            string a = "";
            do
            {
                a = Console.ReadLine();
                if (a != "*")
                {
                    t = Contains(summary_table, a,station_rout);
                    if (t > 0)
                    {
                        st.Add(t);
                        if (user_roat != "всі")
                            Console.WriteLine("якщо ввели всі потрібні станції, введіть *");
                        else break;
                    }
                }
            } while (a != "*");
            return st;
        }
        static bool Time_or_stains()
        {
            char answer = ' ';            
            bool b = false;
            do
            {
                if (b)
                    Console.WriteLine("Помилка введення, повторіть спробу");
                Console.WriteLine("1 Бажаєте відфільтрувати за часом\n2 Бажаєте відфільтрувати за зупинками");
                int choice = Convert.ToInt32(Console.ReadLine());               
                if (choice == 1) return true;
                else if (choice == 2)return false;
                else answer = '@';
            b = true;
            } while (answer == '@');
            return true;
        }
        static void Protection_for_time(ref int start_time,ref int end_time)
        {
            Regex reg_time = new Regex(@"\d\d:\d\d-\d\d:\d\d");
            bool b = false;
            string user_time = "";
            do
            {
                if (b)
                    Console.WriteLine("Помилка, перевірте правельність введеного");
                user_time = Console.ReadLine();
                if (reg_time.IsMatch(user_time))
                {
                    string[] time = user_time.Split('-');
                    start_time = Convert.ToInt32(time[0].Remove(2,1));
                    end_time = Convert.ToInt32(time[1].Remove(2,1));
                }
                b = true;
            }
            while(start_time > end_time || start_time < 0 || start_time > 2400 || end_time < 0 || end_time > 2400);
            
        }
        static void Time(List<string[]> summary_table, char direction, string user_rout, List<int> station_rout)
        {
            int start_time = -1;
            int end_time = -1;
            Protection_for_time(ref start_time,ref end_time);
            int[] size_time = size_user_time(summary_table, direction, user_rout, station_rout, start_time, end_time);            
            StreamWriter write = new StreamWriter(file_answer);
            
            write.WriteLine("<meta charset=" + "UTF-8" + ">");
            write.WriteLine("<table>");
            for (int j = size_time[0]; j < size_time[1] + 1; j++)
            {                
                write.WriteLine("<td><table border=1>");               
                for (int i = 0; i < station_rout.Count; i++)
                {
                    int p = -1;
                    string colour = "00ccff";
                        if (summary_table[station_rout[i]][j] != "")
                            p = Convert.ToInt32(summary_table[station_rout[i]][j].Remove(2, 1));
                        else colour = "#ffcc00";
                        write.WriteLine("<tr>");
                        if (start_time <= p && p <= end_time)
                        {
                           
                            if (j == size_time[0])
                            {
                                write.WriteLine();
                                write.Write("<td>" + summary_table[station_rout[i]][0] + "</td><td>" + summary_table[station_rout[i]][j] + "</td>");
                            }
                            else write.Write("<td>" + summary_table[station_rout[i]][j] + " </td> ");
                            
                        }
                        else
                        {
                            string time = summary_table[station_rout[i]][j];
                            if (time == "") time = "_____";
                            
                            if (j == size_time[0])
                            {
                                write.WriteLine();
                                write.Write("<td >" + summary_table[station_rout[i]][0] + "</td><td  bgcolor=" + colour + ">" + time + "</td>");                                
                            }
                            else write.Write("<td bgcolor=" + colour + ">" + time + " </td> ");
                            
                        }
                        write.WriteLine("</tr>");
                    }
                
                write.WriteLine("</table></td>");
                }
            
            write.Close();
            
        }
        static int[] size_user_time(List<string[]> summary_table, char direction, string user_rout, List<int> station_rout,int start_time,int end_time)
        {
            int[] size_time = new int[2];
            int p = 0;
            for (int i = 1; i < summary_table[0].Length; i++)            
                for (int j = 0; j < station_rout.Count; j++)
                {
                    if (summary_table[station_rout[j]][i] != "" && summary_table[0][i].Contains(direction) && summary_table[0][i].Remove(0, 1) == user_rout)
                    {
                        p = Convert.ToInt32(summary_table[station_rout[j]][i].Remove(2, 1));
                            if (start_time <= p && p <= end_time && size_time[0] == 0)
                                size_time[0] = i;
                            if (start_time <= p && p <= end_time && size_time[1] < i)
                                size_time[1] = i;                        
                    }
                }
            return size_time;
        }
        static List<int> Station_rout(List<string[]> summary_table, char direction, string user_rout)        
        {
            List<int> stations = new List<int>();
            string[] station = File.ReadAllText(direction + user_rout + ".txt", System.Text.Encoding.Default).Split('\t');
            foreach (string str in station)
	        {
                for (int i = 1; i < summary_table.Count; i++)
                {
                    if (str == summary_table[i][0]) stations.Add(i);
                }
	        }
            return stations;
        }
        static bool Station_rout_bool(List<string[]> summary_table, char direction, string user_rout, int user_station)
        {
            string[] station = File.ReadAllText(direction + user_rout + ".txt", System.Text.Encoding.Default).Split('\t');
            foreach (string str in station)
            {
                for (int i = 1; i < summary_table.Count; i++)
                {
                    if (str == summary_table[user_station][0])
                    {
                        return true;                        
                    }
                }
            }
            return false;
        }
        static void Checking_for_all_rout(List<string[]> summary_table, char direction, string user_rout, List<int> stations, List<int> station_rout,StreamWriter write)
        {
            if (Station_rout_bool(summary_table, direction = '+', user_rout, stations[0]) && Station_rout_bool(summary_table, direction = '-', user_rout, stations[0]))
            {
                Console.WriteLine("Оберіть");
                direction = Choice_direction(user_rout);
                station_rout = Station_rout(summary_table, direction, user_rout);
                Output(summary_table, direction, stations, user_rout, write, true);
            }
            else if (Station_rout_bool(summary_table, direction = '+', user_rout, stations[0]))
            {
                direction = '+';
                station_rout = Station_rout(summary_table, direction, user_rout);
                Output(summary_table, direction, stations, user_rout, write, true);
            }
            else if (Station_rout_bool(summary_table, direction = '-', user_rout, stations[0]))
            {
                direction = '-';
                station_rout = Station_rout(summary_table, direction, user_rout);
                Output(summary_table, direction, stations, user_rout, write, true);
            }
        }
        static void Main(string[] args)
        {
            List<string[]> summary_table = new List<string[]>();
            //масив для збереження зведеної таблиці
            List<int> station_rout = new List<int>();
            //масив для збереження номерів рядків всіх зупинок зупинок введеного маршруту (із зведеної таблиці)
            List<int> stations = new List<int>();
            //масив для збереження номерів рядків введених зупинок (із зведеної таблиці)            
            string user_rout = "";
            //змінна в якій зберігається обраний користувачем маршрут
            char direction = '@';
            //змінна в якій зберігається обраний користувачем напрямок маршрут (в вигляді @ або +,або -)
            Input(summary_table);
            Console.WriteLine("Оберіть маршрут");
            user_rout = Сhoice_rout();
            if (user_rout != "всі")
            {
                Console.WriteLine("Оберіть");
                direction = Choice_direction(user_rout);
            }
            station_rout = Station_rout(summary_table, direction, user_rout);
            if (user_rout != "всі")
            {
                Console.WriteLine("Оберіть");
                if (!Time_or_stains())
                {
                    Console.WriteLine("Введіть назву зупинки");
                    stations = Search_Stains(summary_table, station_rout, user_rout);
                    StreamWriter write = new StreamWriter(file_answer);
                    write.WriteLine("<meta charset=" + "UTF-8" + ">");
                    write.WriteLine("<table>");
                    write.WriteLine("<tr>");
                    Output(summary_table, direction, stations, user_rout, write, false);
                    write.WriteLine("</tr>");
                    write.Close();
                }
                    
                  else
                    {
                        Console.WriteLine("Введіть час в форматі ##:##-##:## (замість # вводити час)");
                        Time(summary_table, direction, user_rout, station_rout);
                    }
                
            }
            else
            {
                Console.WriteLine("Введіть назву зупинки");
                stations = Search_Stains(summary_table, station_rout, user_rout);
                string[] st = File.ReadAllText(rout, System.Text.Encoding.Default).Split(' ');
                StreamWriter write = new StreamWriter(file_answer);
                write.WriteLine("<meta charset=" + "UTF-8" + ">");
                write.WriteLine("<table>");
                write.WriteLine("<tr>");
                for (int i = 0; i < st.Count() - 1; i++)
                {
                    user_rout = st[i];
                    Checking_for_all_rout(summary_table, direction, user_rout, stations, station_rout, write);
                }
                write.WriteLine("</tr>");
                write.Close();
            }

            Console.ReadKey();
            System.Diagnostics.Process.Start("output.html");
        }
    }
}