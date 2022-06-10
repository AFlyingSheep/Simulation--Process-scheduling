using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS_2
{
    public partial class Form1 : Form
    {
        int[] a;
        public Form1()
        {
            InitializeComponent();
            a = new int[100];
            for (int i = 0; i < 100; i++)
            {
                a[i] = 0;
            }
            comboBox1.SelectedIndex = 0;
            listView1.ListViewItemSorter = new Order();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(this.listView1, a);
            f.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int s = int.Parse(item.Text);
                listView1.Items.Remove(item);
                a[s - 1] = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int item = comboBox1.SelectedIndex;
            int process_count = listView1.Items.Count;

            if (process_count == 0)
            {
                MessageBox.Show("请输入进程！");
                return;
            }

            Node[] nodes = new Node[process_count];
            for (int i = 0; i < process_count; i++)
            {
                int index = int.Parse(this.listView1.Items[i].SubItems[0].Text);
                int arrive = int.Parse(this.listView1.Items[i].SubItems[1].Text);
                int service = int.Parse(this.listView1.Items[i].SubItems[2].Text);
                nodes[i] = new Node(arrive, service, index);
            }

            mainFrame mf = new mainFrame(item, nodes, process_count);
            mf.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new FileOpen().readFile(listView1, a);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            for (int i = 0; i < a.Length; i++) a[i] = 0;
        }
    }

    public class Node
    {
        public int arrive_time;
        public int service_time;
        public int index;
        public Node(int arrive_time, int service_time, int index)
        {
            this.arrive_time = arrive_time;
            this.service_time = service_time;
            this.index = index;
        }

    }
}
