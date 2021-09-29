using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;

namespace RNPcollection
{
    class Program
    {
        static void Main(string[] args)
        {
            LogWriter log = new LogWriter("");
            log.LogWrite("Начало сбора данных из РНП");

            DateTime startDate = new DateTime(2014, 01, 01); // объявление начальной даты периода поиска
            DateTime endDate = new DateTime(2015, 01, 01); // объявление конечной даты периода поиска

            DataTable dt_result = new DataTable(); // объявление таблицы, в которую будет складываться результат
            dt_result.Columns.Add("inn", typeof(String));
            dt_result.Columns.Add("link", typeof(String));

            while (startDate < DateTime.Now) {
                int step = 365; // устаналиваем шаг в периоде 365 дней
                int pageNum = 1; // устанавливаем номер страницы на 1
                // делаем запрос
                string result = GetHTMLDocumentAsString(startDate, endDate, pageNum);
                // получаем количество результатов
                String ResultCount = Regex.Match(result, @"(?<=class=""search-results__total"">)[\d\s]+(?=записей)|(?<=class=""search-results__total"">более)[\d\s]+(?=записей)").Value.ToString().Trim().Replace(" ", "");
                // проверяем количество результатов 0 или больше
                if (ResultCount.Length > 0)
                {
                    // результатов больше 0
                    log.LogWrite("Получен новый период с " + startDate.ToShortDateString() + " по " + endDate.ToShortDateString() + " с количеством записей " + ResultCount);
                    // делаем цикл с уменьшением шага в периоде до тех пор пока результатов не станет меньше 1000
                    while (Convert.ToInt32(ResultCount) >= 1000) {
                        step -= 20;
                        endDate = startDate.AddDays(step);
                        result = GetHTMLDocumentAsString(startDate, endDate, pageNum);
                        ResultCount = Regex.Match(result, @"(?<=class=""search-results__total"">)[\d\s]+(?=записей)|(?<=class=""search-results__total"">более)[\d\s]+(?=записей)").Value.ToString().Trim().Replace(" ", "");
                    }
                    // получаем количество страниц с записями
                    int pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ResultCount) / 100));
                    log.LogWrite("Начата обработка периода с " + startDate.ToShortDateString() + " по " + endDate.ToShortDateString() + " с количеством записей " + ResultCount);

                    // цикл по всем страницам в указанном периоде
                    for (int i = 1; i <= pageCount; i++) {
                        result = GetHTMLDocumentAsString(startDate, endDate, i);
                        MatchCollection INNmatches = Regex.Matches(result, @"(?<=<div class=""registry-entry__body-title"">ИНН \(аналог ИНН\)<\/div><div class=""registry-entry__body-value"">).{0,4}[\d]+");
                        MatchCollection URLmatches = Regex.Matches(result, @"(?<=class=""registry-entry__header-mid__number""><ahref="").*?(?=\"")");
                        if (i == pageCount)
                        {
                            if (INNmatches.Count == (Convert.ToInt32(ResultCount) - (pageCount - 1) * 100) && URLmatches.Count == (Convert.ToInt32(ResultCount) - (pageCount - 1) * 100))
                            {
                                for (int row = 0; row < INNmatches.Count; row++)
                                {
                                    dt_result.Rows.Add(new Object[] { INNmatches[row].ToString(), URLmatches[row].ToString() });
                                }
                            }
                            else {
                                log.LogWrite("За период с " + startDate.ToShortDateString() + " по " + endDate.ToShortDateString() + " не удалось собрать данные со страницы " + i);
                            }
                        }
                        else 
                        {
                            if (INNmatches.Count == 100 && URLmatches.Count == 100)
                            {
                                for (int row = 0; row < INNmatches.Count; row++)
                                {
                                    dt_result.Rows.Add(new Object[] { INNmatches[row].ToString(), URLmatches[row].ToString() });
                                }
                            }
                            else {
                                log.LogWrite("За период с " + startDate.ToShortDateString() + " по " + endDate.ToShortDateString() + " не удалось собрать данные со страницы " + i);
                            }
                        }
                    }
                    step = 365; // устаналиваем шаг в периоде 365 дней
                    startDate = endDate.AddDays(1); // меняем начальную дату на следующий, после прошлой даты окончания, день
                    endDate = startDate.AddDays(step);  // меняем конечную дату на шаг в периоде от начальной даты
                }
                else 
                {
                    // результатов 0
                    // меняем начальную дату на следующий, после прошлой даты окончания, день
                    startDate = endDate.AddDays(1);
                    // меняем конечную дату на шаг в периоде от начальной даты
                    endDate = startDate.AddDays(step);
                }
            }
            log.LogWrite("Окончание сбора данных из РНП");

        }
        private static String GetHTMLDocumentAsString(DateTime startDate, DateTime endDate, int pageNum)
        {
            // Метод для получения HTMLDoc в виде строки
            // Входящие данные:
            // - начальная дата запроса
            // - конечная дата запроса
            // - номер страницы
            string url = "https://zakupki.gov.ru/epz/dishonestsupplier/search/results.html?morphology=on&search-filter=%D0%94%D0%B0%D1%82%D0%B5+%D1%80%D0%B0%D0%B7%D0%BC%D0%B5%D1%89%D0%B5%D0%BD%D0%B8%D1%8F&sortBy=UPDATE_DATE" +
                    "&pageNumber=" + pageNum.ToString() + "&sortDirection=false&recordsPerPage=_100&showLotsInfoHidden=false&fz94=on&fz223=on&ppRf615=on&inclusionDateFrom=" +
                    startDate.ToString("dd.MM.yyyy") + "&inclusionDateTo=" + endDate.ToString("dd.MM.yyyy");

            WebRequest reqGET = System.Net.WebRequest.Create(url);
            WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new System.IO.StreamReader(stream);
            string result = sr.ReadToEnd().Replace(Environment.NewLine, "").Replace("  ", "").Replace("\n", "").Replace("\r", "");

            reqGET = null;
            resp = null;
            stream = null;
            sr = null;

            return result;
        }
        public class LogWriter
        {
            private string m_exePath = string.Empty;
            public LogWriter(string logMessage)
            {
                LogWrite(logMessage);
            }
            public void LogWrite(string logMessage)
            {
                m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                try
                {
                    using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                    {
                        Log(logMessage, w);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            public void Log(string logMessage, TextWriter txtWriter)
            {
                try
                {
                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("  :{0}", logMessage);
                    txtWriter.WriteLine("-------------------------------");
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
