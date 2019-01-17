using System;

namespace SortingMainWindow
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Init()
        {
            metroComboBox1.SelectedIndex = 0;
            Core.CoreInit(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Core.currentSort = (Core.SORT_METHOD) metroComboBox1.SelectedIndex;
            Core.ArrayLen = (int) numericUpDown1.Value;
            Core.RunThread(Core.RunSort);
        }
    }
}
