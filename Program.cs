using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;


namespace FileStreamTusk
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string import = Choice();
            string export = Choice();
            Console.WriteLine("Путь к исходному текстовому файлу: " + import);
            Console.WriteLine("Путь к исходному текстовому файлу: " + export);

            const int BUFFER_SIZE = 8192;

            using (var source = File.OpenRead(import)) 
            using (var dest = File.Create(export))
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead;

                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dest.Write(buffer, 0, bytesRead);
                }
            }
            Console.WriteLine("Копирование завершено!");



        }

        public static string Choice()
        {
            try
            {
                string dirPath = Path.Combine(Directory.GetCurrentDirectory());
                DirectoryInfo di = new DirectoryInfo(dirPath);
                DirectoryInfo newDirPath = new DirectoryInfo(dirPath);


                string newdir = Path.Combine(Directory.GetCurrentDirectory());
                string fileName;

                string nd = "";

                while (true)
                {
                    while (true) // проверка ввода 
                    {
                        Console.WriteLine("Для перехода в корневую директорию нажмите <..>, для перехода к папке введите ее название. Для выхода нажмите enter ");
                        Console.WriteLine($"Текущая дирректория: {newDirPath}");
                        Show(newDirPath);
                        Console.Write("Введите команду: ");
                        nd = Console.ReadLine().Trim();

                        ///Проверка существования папки
                        if (nd != null)
                        {
                            string newdirExist = Path.Combine(newdir, nd);
                            DirectoryInfo newDirExist = new DirectoryInfo(newdirExist);

                            /// Если существует, перейти к этой директории
                            if (newDirExist.Exists)
                            {
                                if (nd != "" && nd != "..")
                                {
                                    newdir = Path.Combine(newdir, nd);
                                    newDirPath = new DirectoryInfo(newdir);
                                }
                                else if (nd == ".." && newDirPath.Parent != null)
                                {
                                    newDirPath = newDirPath.Parent;
                                    newdir = newDirPath.FullName;
                                }
                                else if (nd == "" || nd == null) { break; }
                                else { Console.WriteLine("Введено некоректное значение"); }
                                Console.Clear();
                            }

                            /// Введено имя файла? 
                            else if (File.Exists(Path.Combine(newDirPath.FullName, nd)))
                            {
                                fileName = Path.Combine(newDirPath.FullName, nd);
                                return fileName;
                            }

                            else { Console.WriteLine("Такого файла или папки не существует!"); continue; }
                        }
                    }
                                    
                }
            }
            catch (IOException ex) { return "\nОшибка ввода-вывода: " + ex.Message; }
            catch (UnauthorizedAccessException ex) { return "\nНедостаточно прав: " + ex.Message; }
            catch (Exception ex) { return "\nНеизвестная ошибка: " + ex; }

        }

        public static void Show(DirectoryInfo newDirPath)
        {
            List<FileInfo> dirInfo = new List<FileInfo>();
            foreach (DirectoryInfo subDir in newDirPath.GetDirectories()) // вывод списка папок
            {
                Console.WriteLine($"\n\t - *{subDir.Name}\n");
            }

            foreach (FileInfo fi in newDirPath.GetFiles()) // вывод списка файлов
            {
                dirInfo.Add(fi);
                Console.WriteLine($"\t - {fi.Name}\n");
            }
        }






    }
}