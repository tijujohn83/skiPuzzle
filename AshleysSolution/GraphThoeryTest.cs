using System;
using System.Net;
using System.Threading.Tasks;


namespace SnakeAndLadders
{
   
    class GraphThoeryTest
    {
       public void TestSki()
        {
            SingaporeSki obj = new SingaporeSki(4);
            int[][] input = new int[4][];
            input[0] = new int[] { 4, 8, 7, 3};
            input[1] = new int[] { 3, 4, 9, 3 };
            input[2] = new int[] { 2, 5, 8, 9};
            input[3] = new int[] { 1, 6, 7, 10 };
            int[] sdinput = new int[17];
            obj.CreateAdjacencyList(input);
            int c = 1;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    
                    sdinput[c] = input[i][j];
                    c++;
                }
                   
            }
            obj.DFS(new int[17], new bool[17],sdinput);

            
        }

       
        public void TestskiOnline()
        {
            var task = GetData("http://s3-ap-southeast-1.amazonaws.com/geeks.redmart.com/coding-problems/map.txt");
            string str = task.Result;
            string[] result = str.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
 
            SingaporeSki obj = new SingaporeSki(1000);
            int[][] input = new int[1000][];
            for (int i = 1; i <= 1000; i++)
            {
                input[i - 1] = Array.ConvertAll(result[i].Split(' '), Int32.Parse);
            }
            obj.CreateAdjacencyList(input);
            int[] sdinput = new int[1000*1000+1];
            int c = 1;
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                    sdinput[c] = input[i][j];
                    c++;
                }

            }
            obj.DFS(new int[1000 * 1000 + 1], new bool[1000 * 1000 + 1],sdinput);

          
        }

        private async Task<string> GetData(string address)
        {
            WebClient client = new WebClient();
            var task = await client.DownloadStringTaskAsync(address);
            return task;
        }

    }
}
