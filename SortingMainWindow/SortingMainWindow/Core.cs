using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SortingMainWindow
{
    public static class Core
    {
        public enum SORT_METHOD
        {
            BUBBLESORT = 0
        }

        #region Declatarion Stuff
        private static Dictionary<double, Thread> threadPool = new Dictionary<double, Thread>();
        internal static Form1 main = null;
        private static ManualResetEvent syncEvent = new ManualResetEvent(false);
        internal static SORT_METHOD currentSort = SORT_METHOD.BUBBLESORT;
        internal static int ArrayLen = 1;

        #endregion
        public static Thread RunThread(Action methodName)
        {
            ManualResetEvent syncEvent = new ManualResetEvent(false);
            double unixMilli = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            Thread newThread = new Thread(
        () =>
        {
            syncEvent.Set();
            methodName();
            syncEvent.WaitOne();
            threadPool.Remove(unixMilli);
        }

    );
            if (threadPool.ContainsKey(unixMilli))
            {
                Thread.Sleep(5);
                threadPool.Add(unixMilli, newThread);
            }

            newThread.Start();
            return newThread;
        }
        public static void CoreInit(Form1 frm)
        {
            main = frm;
        }
        public static void CloseAllThreads()
        {
            foreach (double key in threadPool.Keys)
            {
                threadPool[key].Abort();
            }
        }
        public static void RunSort()
        {
            int[] myArray = new int[ArrayLen];
            int arrayAccesses = 0, comparisons = 0;

            FillArray(ref myArray);

            switch (currentSort)
            {
                case SORT_METHOD.BUBBLESORT:
                    Invoker.SetLabelText(main.label1, myArray.Length.ToString());
                    for (int n = myArray.Length; n > 1; n--)
                    {
                        for (int i = 0; i < n - 1; i = i + 1)
                        {
                            int tmp = myArray[i+1];
                            int cur = myArray[i];
                            arrayAccesses += 2;
                            if (cur > tmp)
                            {
                                myArray[i] = tmp;
                                myArray[i + 1] = cur;
                                arrayAccesses += 2;
                            }
                            comparisons++;
                        }
                        Invoker.SetLabelText(main.label1, "Array Access: " + arrayAccesses.ToString() + Environment.NewLine + "Comparison: " + comparisons.ToString());
                    }
                   
                    PrintArray(myArray, main.richTextBox1);
                    break;
            }

        }
        private static void PrintArray(int[] arr, Control ctrl)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i<arr.Length; i++)
            {
                str.AppendLine(arr[i].ToString());
            }
            Invoker.ChangeText(ctrl, str.ToString());
        }

        private static void FillArray(ref int[] arr)
        {
            int max = arr.Length + 1;
            Random rnd = new Random();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(0, max);
            }
        }
    }
}
