using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ETL;
using PageFetcher;
using ImageFetcher;
using WhatsRent.Crawler.RentPageFetcher;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunETL();
            RunCrawl(args);
            //RunImages(args);
        }

        static void RunImages(string[] args)
        {
            if(args.Length == 0)
                CF_Builders.RunCFBuilders();

            else
            {
                switch (int.Parse(args[0]))
                {
                    case 0: PT_Images.RunPTImages(); break;
                    case 1: CF_Images.RunCFImages(); break;
                    case 2: CF_Builders.RunCFBuilders(); break;
                }
            }
        }

        static void RunETL()
        {
            //Acres99ETL.ParseBrokers();
            //ETLPTProject.ParseProjects();

        }

        static void RunCrawl(string[] args)
        {
            //MagicBricks.RunMagicBricks(args);
            //Acres99.Run99Acres(args);
            //JustDial.RunJustDial(args);
            //PropTiger.RunPropTiger(args);
            CommonFloorRent.RunCommonFloor(args);
            //CommonFloor.Experiment(args);
        }
    }
}
