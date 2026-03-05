using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime;
using System.Text;


namespace FileStreamTusk
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string import = Choice(); // возвращает стринг 
            string export = Choice();
            Console.WriteLine("Путь к исходному текстовому файлу: " + import);
            Console.WriteLine("Путь к исходному текстовому файлу: " + export);

            const int BUFFER_SIZE = 8192;
            int lineNumber = 0;
            int errorCount = 0;

            using (var reader = new StreamReader(import, Encoding.UTF8))
            {
                using (var writer = new StreamWriter(export, false, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        // Заменяем все вхождения (регистронезависимо)
                        string processed = line;
                        int replacementsThisLine = 0;

                        // Простой способ посчитать и заменить (можно улучшить позже)
                        while (processed.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            processed = processed.Replace("ERROR", "ОШИБКА", StringComparison.OrdinalIgnoreCase);
                            replacementsThisLine++;
                        }

                        errorCount += replacementsThisLine;

                        lineNumber++;
                        writer.WriteLine($"[{lineNumber:000000}] {processed}");

                        // Прогресс (опционально, можно убрать)
                        if (lineNumber % 500 == 0)
                            Console.Write(".");
                    }

                    // Итоговая строка в конце файла
                    writer.WriteLine("");
                    writer.WriteLine($"Слово ERROR (или error) встречено {errorCount} раз");
                }

                Console.WriteLine();
                Console.WriteLine($"Обработано строк: {lineNumber}");
                Console.WriteLine($"Заменено вхождений ERROR: {errorCount}");
            }

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