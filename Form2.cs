using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS_2
{
    
    public partial class Form2 : Form
    {
        int[] a;
        ListView listView;
        public Form2(ListView listView, int[] a)
        {
            InitializeComponent();
            this.listView = listView;
            this.a = a;
        }

        private int countColcu(int[] a)
        {
            int count = -1;
            for (int i = 0; i < 100; i++)
            {
                if (a[i] == 0)
                {
                    a[i] = 1;
                    count = i + 1;
                    break;
                }
            }
            return count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = countColcu(a);
            if (count == -1)
            {
                MessageBox.Show("当前进程达到上限！请删除后在添加。");
            }
            if (textBox1.Text == null)
            {
                MessageBox.Show("请输入到达时间！");
                return;
            }
            if (textBox2.Text == null)
            {
                MessageBox.Show("请输入服务时间！");
                return;
            }
            int arrive = int.Parse(textBox1.Text);
            int service = int.Parse(textBox2.Text);

            ListViewItem listViewItem = listView.Items.Add(count.ToString());
            listViewItem.SubItems.Add(arrive.ToString());
            listViewItem.SubItems.Add(service.ToString());
            this.Close();
        }
    }

    class FileOpen
    {
        private string selectPath()
        {
            string path = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Files (*.txt)|*.txt"//如果需要筛选txt文件（"Files (*.txt)|*.txt"）
            };

            //var result = openFileDialog.ShowDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }

            return path;
        }

        private int countColcu(int[] a)
        {
            int count = -1;
            for (int i = 0; i < 100; i++)
            {
                if (a[i] == 0)
                {
                    a[i] = 1;
                    count = i + 1;
                    break;
                }
            }
            return count;
        }

        public static bool digitjdg(string x)
        {
            const string pattern = "^[0-9]*$";
            Regex rx = new Regex(pattern);
            bool IsDigit = rx.IsMatch(x);
            return IsDigit;//是数字返回true,不是返回false
        }

        public bool errorDetect(string[] s)
        {
            if (s.Length != 2) return false;
            foreach (string str in s) {
                if (!digitjdg(str)) return false;
            }
            return true;
        }
        public void readFile(ListView listView, int[] a)
        {
            string path = selectPath();
            if (path == String.Empty)
            {
                MessageBox.Show("读取失败！");
                return;
            }
            StreamReader sr = new StreamReader(path, Encoding.Default);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] s = line.Split('\t');

                if (!errorDetect(s))
                {
                    MessageBox.Show("读取错误！请检查后输入。");
                }
                
                ListViewItem listViewItem = listView.Items.Add(countColcu(a).ToString());
                listViewItem.SubItems.Add(s[0]);
                listViewItem.SubItems.Add(s[1]);
            }
            MessageBox.Show("读取成功！");
        }
    }
}
