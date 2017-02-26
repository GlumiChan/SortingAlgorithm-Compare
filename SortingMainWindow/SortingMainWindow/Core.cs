using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace SortingMainWindow
{
    public static class Core
    {
        public enum SORT_METHOD
        {
            BUBBLESORT = 0,
            INSERTIONSORT = 1,
            MERGESORT = 2,
            SELECTIONSORT = 3
        }

        #region Declatarion Stuff
        private static Dictionary<double, Thread> threadPool = new Dictionary<double, Thread>();
        internal static Form1 main = null;
        private static ManualResetEvent syncEvent = new ManualResetEvent(false);
        internal static SORT_METHOD currentSort = SORT_METHOD.BUBBLESORT;
        internal static int ArrayLen = 2;
        internal static int arrayAccesses = 0, comparisons = 0;
        private static Stopwatch sw = new Stopwatch();

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
            arrayAccesses = 0;
            comparisons = 0;

            FillArray(ref myArray);

            sw.Reset();
            sw.Start();
            switch (currentSort)
            {
                case SORT_METHOD.BUBBLESORT:
                    BubbleSort(ref myArray);
                    break;
                case SORT_METHOD.INSERTIONSORT:
                    InsertionSort(ref myArray);
                    break;
                case SORT_METHOD.MERGESORT:
                    MergeSort(ref myArray);
                    break;
                case SORT_METHOD.SELECTIONSORT:
                    SelectionSort(ref myArray);
                    break;
            }
            UpdateLabel(main.label1, true);
            sw.Stop();
            PrintArray(myArray, main.richTextBox1);

        }
        private static void InsertionSort(ref int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int tmp = array[i];
                arrayAccesses++;

                int j = i;
                while (j > 0 && array[j - 1] > tmp)
                {
                    comparisons++;
                    arrayAccesses++;

                    array[j] = array[j - 1];
                    arrayAccesses += 2;

                    j--;
                }
                array[j] = tmp;
                arrayAccesses++;

                UpdateLabel(main.label1);
            }
        }
        private static void BubbleSort(ref int[] array)
        {
            int maxLen = array.Length;

            for (int i = maxLen; i > 1; i--)
            {
                for (int ii = 0; ii < i - 1; ii++)
                {
                    int tmp = array[ii + 1];
                    arrayAccesses++;

                    int cur = array[ii];
                    arrayAccesses++;

                    if (cur > tmp)
                    {
                        array[ii] = tmp;
                        arrayAccesses++;

                        array[ii + 1] = cur;
                        arrayAccesses++;
                    }
                    comparisons++;
                    UpdateLabel(main.label1);
                }
                UpdateLabel(main.label1);
            }
        }
        private static void SelectionSort(ref int[] array)
        {
            int maxLen = array.Length;

            for (int i = 0; i < maxLen; i++)
            {
                int minPos = i;

                for (int ii = minPos + 1; ii < maxLen; ii++)
                {
                    if (array[ii] < array[minPos])
                    {
                        minPos = ii;
                    }
                    arrayAccesses += 2;
                    comparisons++;
                    UpdateLabel(main.label1);
                }

                int firstItem = array[i];
                arrayAccesses++;

                array[i] = array[minPos];
                arrayAccesses += 2;
                array[minPos] = firstItem;
                arrayAccesses++;
                UpdateLabel(main.label1);
            }
        }
        private static void MergeSort(ref int[] array)
        {
            if (array.Length > 1)
            {
                int mid = Convert.ToInt32(array.Length / 2);

                int[] leftSide = new int[mid];

                for (int i = 0; i <= leftSide.Length - 1; i++)
                {
                    leftSide[i] = array[i];
                    arrayAccesses += 2;
                    UpdateLabel(main.label1);
                }

                int[] rightSide = new int[array.Length - mid];

                int maxLen = array.Length;

                for (int i = mid; i <= maxLen - 1; i++)
                {
                    rightSide[i - mid] = array[i];
                    arrayAccesses += 2;
                    UpdateLabel(main.label1);
                }

                MergeSort(ref leftSide);
                MergeSort(ref rightSide);

                array = MergeMerge(ref leftSide, ref rightSide);
            }
        }
        private static int[] MergeMerge(ref int[] leftSide, ref int[] rightSide)
        {
            int[] newArr = new int[leftSide.Length + rightSide.Length];
            arrayAccesses += 2;

            int indexLeft = 0;
            int indexRight = 0;
            int indexResult = 0;

            int leftLen = leftSide.Length;
            int rightLen = rightSide.Length;

            while (indexLeft < leftLen && indexRight < rightLen)
            {
                if (leftSide[indexLeft] < rightSide[indexRight])
                {
                    newArr[indexResult] = leftSide[indexLeft];
                    arrayAccesses += 2;
                    indexLeft += 1;
                }
                else
                {
                    newArr[indexResult] = rightSide[indexRight];
                    arrayAccesses += 2;
                    indexRight += 1;
                }
                comparisons++;

                indexResult += 1;
                UpdateLabel(main.label1);
            }

            while (indexLeft < leftLen)
            {
                newArr[indexResult] = leftSide[indexLeft];
                arrayAccesses += 2;
                indexLeft += 1;
                indexResult += 1;
                UpdateLabel(main.label1);
            }

            while (indexRight < rightLen)
            {
                newArr[indexResult] = rightSide[indexRight];
                arrayAccesses += 2;
                indexRight += 1;
                indexResult += 1;
                UpdateLabel(main.label1);
            }

            return newArr;
        }
        private static void UpdateLabel(Label label, bool force = false)
        {
            if (force)
            {
                if (main.checkBox1.Checked)
                {
                    Invoker.SetLabelText(label, "Array Access: " + arrayAccesses.ToString() + Environment.NewLine + "Comparison: " + comparisons.ToString());
                }else
                {
                    Invoker.SetLabelText(label, "Array Access: " + arrayAccesses.ToString() + Environment.NewLine + "Comparison: " + comparisons.ToString() + Environment.NewLine + "Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
                }
            }else if (!main.checkBox1.Checked)
            {
                Invoker.SetLabelText(label, "Array Access: " + arrayAccesses.ToString() + Environment.NewLine + "Comparison: " + comparisons.ToString() + Environment.NewLine + "Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
            }
        }
        private static void PrintArray(int[] arr, Control ctrl)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
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
