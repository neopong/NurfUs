using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NurfUs.Models;
using NurfUs.Models.API;
using NurfUs.Classes;
using NurfUsMatchPuller.Exceptions;
using System.Diagnostics;

namespace NurfUsMatchPuller
{
    class Program
    {
        private const string DevKey = "apiKey";
        private const string BaseUrl = "https://na.api.pvp.net/api/lol/";
        private const string GlobalBaseUrl = "https://global.api.pvp.net/api/lol/";
        private const long StartEpochTime = 1427865900;
        private const long Interval = 300;

        private static List<Task> TaskList = new List<Task>();
        
        private static readonly string JSONCacheDirectory = ConfigurationManager.AppSettings[ConfigConstants.CONFIGKEY_JSONCACHEDIRECTORY];
        private static readonly string MatchDetailDirectory = ConfigurationManager.AppSettings[ConfigConstants.CONFIGKEY_MATCHDETAILSUBDIRECTORY];

        private static long CurrentEpochTime =
            Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

        internal delegate Task APIDataScraperAsync(string[] args);

        internal static Dictionary<String, APIDataScraperAsync> ScraperFunctions;

        static void Main(string[] args)
        {
            if (!APITaskDictionaryInitialize(out ScraperFunctions))
            {
                throw new ScraperFunctionInitializationException("Program.cs");
            }
            try
            {
                if (ScraperFunctions.Count > 0)
                {
                    foreach (var scraperFunction in ScraperFunctions)
                    {
                        //TaskList.Add(scraperFunction.Value(args));
                        //Upon second thought it's probably not best for both functions to run 
                        //simultaneously because the Match Info is dependent on the results of the match Ids
                        Task t = scraperFunction.Value(args);
                        t.Wait();
                    }
                    //Task.WaitAll(TaskList.ToArray());
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    if (exception.InnerException is System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        System.Data.Entity.Validation.DbEntityValidationException dbException =
                            (System.Data.Entity.Validation.DbEntityValidationException)exception.InnerException;

                        foreach (var eve in dbException.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(exception.Message);

                        if (exception.InnerException != null)
                        {
                            Console.WriteLine(exception.InnerException.Message);
                        }
                    }
                }
            }
            Console.WriteLine("Done");
        }

        static async Task MatchDetailScraperAsync(String[] args)
        {
            /*
             * For the sake of "Compartimentilization" (or however you spell it...)
             * I am assigning these local variables from the global ones, so when
             * this function moves you can easily change what the values are.
             */
            int HttpResponse_RateLimitExceeded = 429;

            String jsonCacheDirectory = JSONCacheDirectory;
            String matchInfoDirectory = MatchDetailDirectory;
            String baseUrl = BaseUrl;
            String fileSearchPattern = "*.json";

            int millisecondsPerRequest = 250;
            //Stopwatch sw = new Stopwatch();
            //====--------------------------------

            List<string> regions = new List<string>();
            
            regions.Add("na");
            regions.Add("euw");
            regions.Add("kr");
            regions.Add("tr");
            regions.Add("lan");
            regions.Add("eune");
            regions.Add("oce");
            regions.Add("las");
            regions.Add("br");
            regions.Add("ru");

            try
            {
                foreach (string region in regions)
                {
                    String inputDirectory = Path.Combine(jsonCacheDirectory, region);
                    String outputDirectory = Path.Combine(inputDirectory, matchInfoDirectory);

                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    foreach (var file in Directory.GetFiles(inputDirectory, fileSearchPattern))
                    {
                        using (StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open)))
                        {
                            //Way to drunk to use any sort of JSON craziness
                            String[] matchIds = reader.ReadToEnd()
                                .Replace("[", "")
                                .Replace("]", "")
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < matchIds.Length; i++)
                            {

                                string outputFileName = Path.Combine(outputDirectory, matchIds[i] + ".json");

                                if (!File.Exists(outputFileName))
                                {
                                    String apiEndpoint = string.Format(
                                            "{0}{1}/v2.2/match/{2}",
                                            baseUrl.Replace("/na.", "/" + region + "."),
                                            region,
                                            matchIds[i]);

                                    //sw.Start();
                                    RESTResult<MatchDetail> gameListResult = await RESTHelpers.RESTRequest<MatchDetail>(apiEndpoint, "", DevKey, "includeTimeline=true", 5);
                                    //sw.Stop();

                                    if (gameListResult.Success)
                                    {
                                        //int remainingDelay = millisecondsPerRequest - (int)sw.ElapsedMilliseconds;
                                        //if (remainingDelay > 0)
                                        //{
                                        //    Thread.Sleep(remainingDelay);
                                        //}

                                        JavaScriptSerializer jss = new JavaScriptSerializer();
                                        File.WriteAllText(outputFileName, jss.Serialize(gameListResult.ReturnObject));
                                        Console.WriteLine(string.Format("Region {0}, File {1}", region, outputFileName));
                                    }
                                    else if ((int)gameListResult.StatusCode == HttpResponse_RateLimitExceeded)
                                    {
                                        Console.WriteLine("Rate Exceeded, waiting {0}ms before trying again.", millisecondsPerRequest);
                                        //decrement the iterator and try this file again momentarily
                                        i--;
                                        Thread.Sleep(millisecondsPerRequest);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Fail, Http Resonse: {0}", (int)gameListResult.StatusCode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception thrown: " + e);
            }
        }

        static async Task MatchIdScraperAsync(string[] args)
        {
            List<string> regions = new List<string>();

            regions.Add("na");
            regions.Add("euw");
            regions.Add("kr");
            regions.Add("tr");
            regions.Add("lan");
            regions.Add("eune");
            regions.Add("oce");
            regions.Add("las"); 
            regions.Add("br"); 
            regions.Add("ru");

            foreach (string region in regions)
            {
                for (long i = StartEpochTime; i < CurrentEpochTime; i += Interval)
                {
                    string fileName = JSONCacheDirectory + region + @"\" + i.ToString() + ".json";

                    if (!File.Exists(fileName))
                    {
                        RESTResult<List<long>> gameListResult =
                            await
                                RESTHelpers.RESTRequest<List<long>>(
                                    string.Format("{0}{1}/v4.1/game/ids", BaseUrl.Replace("/na.", "/" + region + "."), region), "",
                                    DevKey, "beginDate=" + i.ToString(), 4);

                        if (gameListResult.Success)
                        {
                            JavaScriptSerializer jss = new JavaScriptSerializer();
                            File.WriteAllText(fileName, jss.Serialize(gameListResult.ReturnObject));
                            Console.WriteLine(string.Format("Region {0}, File {1}", region, fileName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Static task dictionary initialization
        /// </summary>
        /// <param name="dict"></param>
        private static bool APITaskDictionaryInitialize(out Dictionary<String, APIDataScraperAsync> dict)
        {
            dict = new Dictionary<String, APIDataScraperAsync>();
            try
            {

                if (Convert.ToBoolean(ConfigurationManager.AppSettings[ConfigConstants.CONFIGKEY_RUNMATCHIDSCRAPER]))
                    dict.Add(ConfigConstants.CONFIGKEY_RUNMATCHIDSCRAPER, MatchIdScraperAsync);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings[ConfigConstants.CONFIGKEY_RUNMATCHINFOSCRAPER]))
                    dict.Add(ConfigConstants.CONFIGKEY_RUNMATCHINFOSCRAPER, MatchDetailScraperAsync);

                //As we add more automation tasks to run,
                //Make sure to add them to the dictionary here.

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

/*

 ░░░░░░░░░▄░░░░░░░░░░░░░░▄░░░░
░░░░░░░░▌▒█░░░░░░░░░░░▄▀▒▌░░░
░░░░░░░░▌▒▒█░░░░░░░░▄▀▒▒▒▐░░░
░░░░░░░▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐░░░
░░░░░▄▄▀▒░▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐░░░
░░░▄▀▒▒▒░░░▒▒▒░░░▒▒▒▀██▀▒▌░░░ 
░░▐▒▒▒▄▄▒▒▒▒░░░▒▒▒▒▒▒▒▀▄▒▒▌░░
░░▌░░▌█▀▒▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐░░
░▐░░░▒▒▒▒▒▒▒▒▌██▀▒▒░░░▒▒▒▀▄▌░
░▌░▒▄██▄▒▒▒▒▒▒▒▒▒░░░░░░▒▒▒▒▌░
▀▒▀▐▄█▄█▌▄░▀▒▒░░░░░░░░░░▒▒▒▐░
▐▒▒▐▀▐▀▒░▄▄▒▄▒▒▒▒▒▒░▒░▒░▒▒▒▒▌
▐▒▒▒▀▀▄▄▒▒▒▄▒▒▒▒▒▒▒▒░▒░▒░▒▒▐░
░▌▒▒▒▒▒▒▀▀▀▒▒▒▒▒▒░▒░▒░▒░▒▒▒▌░
░▐▒▒▒▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▒▄▒▒▐░░
░░▀▄▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▄▒▒▒▒▌░░
░░░░▀▄▒▒▒▒▒▒▒▒▒▒▄▄▄▀▒▒▒▒▄▀░░░
░░░░░░▀▄▄▄▄▄▄▀▀▀▒▒▒▒▒▄▄▀░░░░░
░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▀▀░░░░░░░░
 
 */