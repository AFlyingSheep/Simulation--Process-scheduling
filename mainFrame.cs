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

    public partial class mainFrame : Form
    {
        int mode;
        Node[] nodes;
        public mainFrame(int mode, Node[] nodes, int itemCount)
        {
            InitializeComponent();
            this.nodes = nodes;
            this.mode = mode;
            switch (mode)
            {
                case 0:
                    {
                        this.nowAg.Text = "先来先服务(FCFS)";
                        new FCFS(nodes, logBox, listView1).start_execute();
                        break;
                    }
                case 1:
                    {
                        this.nowAg.Text = "轮转RR(q=1)";
                        new RR(nodes, logBox, listView1).start_execute();
                        break;
                    }
                case 2:
                    {
                        this.nowAg.Text = "最短进程优先(SJF)";
                        new SJF(nodes, logBox, listView1).start_execute();
                        break;
                    }
                case 3:
                    {
                        this.nowAg.Text = "最高响应比优先(HRN)";
                        new HRN(nodes, logBox, listView1).start_execute();
                        
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }

    class FCFS
    {
        Node[] nodes;
        TextBox textbox;
        ListView lv;
        public FCFS(Node[] nodes, TextBox textBox, ListView listView)
        {
            this.nodes = nodes;
            this.textbox = textBox;
            this.lv = listView;
        }

        public void start_execute()
        {
            FCFS_Exexute_unit[] execute_Unit = new FCFS_Exexute_unit[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                execute_Unit[i] = new FCFS_Exexute_unit(nodes[i]);
            }

            Array.Sort(execute_Unit);

            int arrayPoint = 0;
            Queue<FCFS_Exexute_unit> queue = new Queue<FCFS_Exexute_unit>();

            int nowTime = 0;
            int queueTopTime = 0;
            int arrayTime = execute_Unit[0].node.arrive_time;
            Execute_unit[] execute_finish = new Execute_unit[nodes.Length];
            int finish = 0;
            while (queue.Count != 0 || arrayPoint < execute_Unit.Length)
            {
                // 队列为空，说明所有执行完了但是下一个还没到来
                if (queue.Count == 0)
                {
                    nowTime = execute_Unit[arrayPoint].node.arrive_time;
                    execute_Unit[arrayPoint].start_time = nowTime;
                    execute_Unit[arrayPoint].finish_time = nowTime + execute_Unit[arrayPoint].node.service_time;
                    queue.Enqueue(execute_Unit[arrayPoint]);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    queueTopTime = queue.Peek().finish_time;

                    // 更改数组头时间
                    if (++arrayPoint < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                }

                while (queueTopTime > arrayTime && arrayPoint < execute_Unit.Length)
                {
                    nowTime = arrayTime;
                    if (arrayPoint + 1 < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint + 1].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                    queue.Enqueue(execute_Unit[arrayPoint]);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    arrayPoint++;
                }

                while (queueTopTime <= arrayTime && queue.Count > 0)
                {
                    Execute_unit outNode = queue.Dequeue();
                    nowTime = outNode.finish_time;
                    execute_finish[finish++] = outNode;
                    popQueue(nowTime, outNode.node, queue, textbox);
                    if (queue.Count > 0)
                    {
                        queue.ElementAt<Execute_unit>(0).start_time = nowTime;
                        queue.ElementAt<Execute_unit>(0).finish_time = nowTime + queue.ElementAt<Execute_unit>(0).node.service_time;
                        queueTopTime = queue.ElementAt<Execute_unit>(0).finish_time;
                    }

                }
            }
            for (int i = 0; i < execute_finish.Length; i++)
            {
                execute_finish[i].turn_time = execute_finish[i].finish_time - execute_finish[i].node.arrive_time;
                execute_finish[i].turn_with_weight_time = (float)execute_finish[i].turn_time / (float)execute_finish[i].node.service_time;
            }

            for (int i = 0; i < execute_finish.Length; i++)
            {
                ListViewItem list = lv.Items.Add(execute_finish[i].node.index.ToString());
                list.SubItems.Add(execute_finish[i].node.arrive_time.ToString());
                list.SubItems.Add(execute_finish[i].finish_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_with_weight_time.ToString());
            }


        }
        public void intoQueue(int time, Node node, Queue<FCFS_Exexute_unit> queue, TextBox textBox)
        {
            string str = "";
            foreach (var item in queue)
            {
                str += item.node.index.ToString() + " ";
            }
            textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "到达" + Environment.NewLine);
            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);


        }

        public void popQueue(int time, Node node, Queue<FCFS_Exexute_unit> queue, TextBox textBox)
        {
            string str = "";
            foreach (var item in queue)
            {
                str += item.node.index.ToString() + " ";
            }
            if (str == "") str = "null";
            if (queue.Count > 0)
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + "  序号" + queue.Peek().node.index.ToString() + "准备开始" + Environment.NewLine);
            }
            else
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + Environment.NewLine);
            }

            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);

        }
    }

    class SJF
    {
        Node[] nodes;
        TextBox textbox;
        ListView lv;
        public SJF(Node[] nodes, TextBox textBox, ListView listView)
        {
            this.nodes = nodes;
            this.textbox = textBox;
            this.lv = listView;
        }

        public void start_execute()
        {
            FCFS_Exexute_unit[] execute_Unit = new FCFS_Exexute_unit[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                execute_Unit[i] = new FCFS_Exexute_unit(nodes[i]);
            }

            Compare compare = new Compare(0);

            Array.Sort(execute_Unit);

            int arrayPoint = 0;
            //PriorityQueue<FCFS_Exexute_unit> queue = new PriorityQueue<FCFS_Exexute_unit>(compare);
            PriorityQueue queue = new PriorityQueue(compare);

            int nowTime = 0;
            int queueTopTime = 0;
            int arrayTime = execute_Unit[0].node.arrive_time;
            Execute_unit[] execute_finish = new Execute_unit[nodes.Length];
            int finish = 0;
            while (queue.Count != 0 || arrayPoint < execute_Unit.Length)
            {
                // 队列为空，说明所有执行完了但是下一个还没到来
                if (queue.Count == 0)
                {
                    nowTime = execute_Unit[arrayPoint].node.arrive_time;
                    execute_Unit[arrayPoint].start_time = nowTime;
                    execute_Unit[arrayPoint].finish_time = nowTime + execute_Unit[arrayPoint].node.service_time;
                    //queue.Enqueue(execute_Unit[arrayPoint]);
                    queue.Enqueue(execute_Unit[arrayPoint], nowTime);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    queueTopTime = queue.Peek().finish_time;

                    // 更改数组头时间
                    if (++arrayPoint < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                }

                while (queueTopTime > arrayTime && arrayPoint < execute_Unit.Length)
                {
                    nowTime = arrayTime;
                    if (arrayPoint + 1 < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint + 1].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                    queue.Enqueue(execute_Unit[arrayPoint], nowTime);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    arrayPoint++;
                }

                while (queueTopTime <= arrayTime && queue.Count > 0)
                {
                    Execute_unit outNode = queue.Dequeue();
                    nowTime = outNode.finish_time;
                    execute_finish[finish++] = outNode;
                    popQueue(nowTime, outNode.node, queue, textbox);
                    if (queue.Count > 0)
                    {
                        queue.heap[0].start_time = nowTime;
                        queue.heap[0].finish_time = nowTime + queue.heap[0].node.service_time;
                        queueTopTime = queue.heap[0].finish_time;
                    }

                }
            }
            for (int i = 0; i < execute_finish.Length; i++)
            {
                execute_finish[i].turn_time = execute_finish[i].finish_time - execute_finish[i].node.arrive_time;
                execute_finish[i].turn_with_weight_time = (float)execute_finish[i].turn_time / (float)execute_finish[i].node.service_time;
            }

            for (int i = 0; i < execute_finish.Length; i++)
            {
                ListViewItem list = lv.Items.Add(execute_finish[i].node.index.ToString());
                list.SubItems.Add(execute_finish[i].node.arrive_time.ToString());
                list.SubItems.Add(execute_finish[i].finish_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_with_weight_time.ToString());
            }


        }


        public void intoQueue(int time, Node node, PriorityQueue queue, TextBox textBox)
        {
            string str = " ";
            foreach (var item in queue.heap)
            {
                str += item.node.index.ToString() + " ";
            }
            if (str == " ") str = "null";
            textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "到达" + Environment.NewLine);
            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);


        }

        public void popQueue(int time, Node node, PriorityQueue queue, TextBox textBox)
        {
            string str = " ";
            foreach (var item in queue.heap)
            {
                str += item.node.index.ToString() + " ";
            }
            if (str == " ") str = "null";
            if (queue.Count > 0)
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + "  序号" + queue.Peek().node.index.ToString() + "准备开始" + Environment.NewLine);
            }
            else
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + Environment.NewLine);
            }

            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);

        }
    }

    class HRN
    {
        Node[] nodes;
        TextBox textbox;
        ListView lv;
        public HRN(Node[] nodes, TextBox textBox, ListView listView)
        {
            this.nodes = nodes;
            this.textbox = textBox;
            this.lv = listView;
        }

        public void start_execute()
        {
            FCFS_Exexute_unit[] execute_Unit = new FCFS_Exexute_unit[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                execute_Unit[i] = new FCFS_Exexute_unit(nodes[i]);
            }

            Compare compare = new Compare(1);

            Array.Sort(execute_Unit);

            int arrayPoint = 0;
            //PriorityQueue<FCFS_Exexute_unit> queue = new PriorityQueue<FCFS_Exexute_unit>(compare);
            PriorityQueue queue = new PriorityQueue(compare);

            int nowTime = 0;
            int queueTopTime = 0;
            int arrayTime = execute_Unit[0].node.arrive_time;
            Execute_unit[] execute_finish = new Execute_unit[nodes.Length];
            int finish = 0;
            while (queue.Count != 0 || arrayPoint < execute_Unit.Length)
            {
                // 队列为空，说明所有执行完了但是下一个还没到来
                if (queue.Count == 0)
                {
                    nowTime = execute_Unit[arrayPoint].node.arrive_time;
                    execute_Unit[arrayPoint].start_time = nowTime;
                    execute_Unit[arrayPoint].finish_time = nowTime + execute_Unit[arrayPoint].node.service_time;
                    //queue.Enqueue(execute_Unit[arrayPoint]);
                    queue.Enqueue(execute_Unit[arrayPoint], nowTime);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    queueTopTime = queue.Peek().finish_time;

                    // 更改数组头时间
                    if (++arrayPoint < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                }

                while (queueTopTime > arrayTime && arrayPoint < execute_Unit.Length)
                {
                    nowTime = arrayTime;
                    if (arrayPoint + 1 < execute_Unit.Length)
                    {
                        arrayTime = execute_Unit[arrayPoint + 1].node.arrive_time;
                    }
                    else
                    {
                        arrayTime = Int32.MaxValue;
                    }
                    queue.Enqueue(execute_Unit[arrayPoint], nowTime);
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, queue, textbox);
                    arrayPoint++;
                }

                while (queueTopTime <= arrayTime && queue.Count > 0)
                {

                    Execute_unit outNode = queue.Dequeue();
                    nowTime = outNode.finish_time;
                    execute_finish[finish++] = outNode;
                    popQueue(nowTime, outNode.node, queue, textbox);
                    if (queue.Count > 0)
                    {
                        queue.heap[0].start_time = nowTime;
                        queue.heap[0].finish_time = nowTime + queue.heap[0].node.service_time;
                        queueTopTime = queue.heap[0].finish_time;
                    }

                }
            }
            for (int i = 0; i < execute_finish.Length; i++)
            {
                execute_finish[i].turn_time = execute_finish[i].finish_time - execute_finish[i].node.arrive_time;
                execute_finish[i].turn_with_weight_time = (float)execute_finish[i].turn_time / (float)execute_finish[i].node.service_time;
            }

            for (int i = 0; i < execute_finish.Length; i++)
            {
                ListViewItem list = lv.Items.Add(execute_finish[i].node.index.ToString());
                list.SubItems.Add(execute_finish[i].node.arrive_time.ToString());
                list.SubItems.Add(execute_finish[i].finish_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_time.ToString());
                list.SubItems.Add(execute_finish[i].turn_with_weight_time.ToString());
            }


        }


        public void intoQueue(int time, Node node, PriorityQueue queue, TextBox textBox)
        {
            string str = " ";
            foreach (var item in queue.heap)
            {
                str += item.node.index.ToString() + " ";
            }
            if (str == " ") str = "null";
            textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "到达" + Environment.NewLine);
            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);


        }

        public void popQueue(int time, Node node, PriorityQueue queue, TextBox textBox)
        {
            string str = " ";
            foreach (var item in queue.heap)
            {
                str += item.node.index.ToString() + " ";
            }
            if (str == " ") str = "null";
            if (queue.Count > 0)
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + "  序号" + queue.Peek().node.index.ToString() + "准备开始" + Environment.NewLine);
            }
            else
            {
                textbox.AppendText("Time:" + time.ToString() +
                        "  序号" + node.index.ToString() + "完成任务" + Environment.NewLine);
            }

            textbox.AppendText("Time:" + time.ToString() +
                "  此时就绪队列：" + str + Environment.NewLine);
            textbox.AppendText(Environment.NewLine);

        }
    }
    class RR
    {
        Node[] nodes;
        TextBox textbox;
        ListView lv;
        public RR(Node[] nodes, TextBox textBox, ListView listView)
        {
            this.nodes = nodes;
            this.textbox = textBox;
            this.lv = listView;
        }

        public void start_execute()
        {
            RR_Execute_unit[] execute_Unit = new RR_Execute_unit[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                execute_Unit[i] = new RR_Execute_unit(nodes[i]);
                execute_Unit[i].need_time = execute_Unit[i].node.service_time;
            }
            Array.Sort(execute_Unit);

            int nowTime = execute_Unit[0].node.arrive_time;
            /*
            // arrayPoint 指向没到达数组的第一个
            int arrayPoint = 0;

            // diaoduPoint 指向调度队列的最后一个
            int diaoduPoint = 0;
            RR_Execute_unit[] run = new RR_Execute_unit[nodes.Length];
            */
            Queue<RR_Execute_unit> queue = new Queue<RR_Execute_unit>();
            
            int alreadyCount = 0;
            int not_ex_point = 0;
            int finishPoint = 0;
            RR_Execute_unit[] finishUnit = new RR_Execute_unit[nodes.Length];

            while (alreadyCount < nodes.Length)
            {
                if (queue.Count > 0)
                {
                    while (not_ex_point < nodes.Length && nowTime >= execute_Unit[not_ex_point].node.arrive_time)
                    {
                        queue.Enqueue(execute_Unit[not_ex_point]);
                        intoQueue(nowTime, execute_Unit[not_ex_point].node, textbox);
                        not_ex_point++;
                    }

                    aClock(nowTime, queue.Peek().node, textbox);
                    queue.ElementAt(0).need_time--;
                    if (queue.Peek().need_time == 0)
                    {
                        RR_Execute_unit rR_Execute_Unit = queue.Dequeue();
                        rR_Execute_Unit.finish_time = nowTime;
                        finishUnit[finishPoint++] = rR_Execute_Unit;
                        finish(nowTime, rR_Execute_Unit.node, textbox);
                        alreadyCount++;
                    }
                    else
                    {
                        queue.Enqueue(queue.Dequeue());
                    }
                    nowTime++;

                }
                else
                {
                    nowTime = execute_Unit[not_ex_point].node.arrive_time;
                    queue.Enqueue(execute_Unit[not_ex_point]);
                    intoQueue(nowTime, execute_Unit[not_ex_point].node, textbox);
                    not_ex_point++;
                    nowTime++;
                }
            }


            /*int[] res_run_time = new int[nodes.Length];
            bool[] dirty = new bool[nodes.Length];
            int lunzhuan = 0;
            int run_count = 0;
            */
            
            /*int max = nodes.Length;*/


            /*while (run_count < nodes.Length)
            {
                for (int i = 0; i < max; i++) dirty[i] = true;
                if (arrayPoint < max && nowTime >= execute_Unit[arrayPoint].node.arrive_time)
                { 
                    intoQueue(nowTime, execute_Unit[arrayPoint].node, textbox);
                    FCFS_Exexute_unit fCFS = execute_Unit[arrayPoint];
                    run[diaoduPoint] = execute_Unit[arrayPoint];
                    res_run_time[diaoduPoint] = fCFS.node.service_time;
                    dirty[diaoduPoint] = false;
                    diaoduPoint++;
                    arrayPoint++;
                }

                int isFound = 0;
                for (int i = 1; i < diaoduPoint + 10; i++)
                {
                    if (res_run_time[(lunzhuan + i) % (diaoduPoint)] > 0 && dirty[(lunzhuan + i) % (diaoduPoint)])
                    {
                        isFound = 1;
                        lunzhuan = (lunzhuan + i) % (diaoduPoint);
                        break;
                    }
                }

                if (isFound == 1)
                {
                    res_run_time[lunzhuan] -= 1;
                    aClock(nowTime, run[lunzhuan].node, textbox);
                    if (res_run_time[lunzhuan] == 0)
                    {
                        finish(nowTime, run[lunzhuan].node, textbox);
                        run[lunzhuan].finish_time = nowTime;
                        finishUnit[finishPoint++] = run[lunzhuan];
                        run_count++;
                    }

                }
                nowTime++;
            }*/
            for (int i = 0; i < finishUnit.Length; i++)
            {
                finishUnit[i].turn_time = finishUnit[i].finish_time - finishUnit[i].node.arrive_time;
                finishUnit[i].turn_with_weight_time = (float)finishUnit[i].turn_time / (float)finishUnit[i].node.service_time;
            }

            for (int i = 0; i < finishUnit.Length; i++)
            {
                ListViewItem list = lv.Items.Add(finishUnit[i].node.index.ToString());
                list.SubItems.Add(finishUnit[i].node.arrive_time.ToString());
                list.SubItems.Add(finishUnit[i].finish_time.ToString());
                list.SubItems.Add(finishUnit[i].turn_time.ToString());
                list.SubItems.Add(finishUnit[i].turn_with_weight_time.ToString());
            }
        }
      

        public void intoQueue(int time, Node node, TextBox textBox)
        {
            textbox.AppendText("Time:" + time.ToString() +
                "  序号：" + node.index + "加入调度！" + Environment.NewLine);
        }

        public void aClock(int time, Node node, TextBox textBox)
        {
            textbox.AppendText("Time:" + time.ToString() +
                "  序号：" + node.index + "运行一个时间片！" + Environment.NewLine);
        }
        public void finish(int time, Node node, TextBox textBox)
        {
            textbox.AppendText("Time:" + time.ToString() +
                "  序号：" + node.index + "运行完成！" + Environment.NewLine);
        }
    }
    class Execute_unit
    {
        public int start_time;
        public int finish_time;
        public int turn_time;
        public float turn_with_weight_time;
        public Node node;
        public int nowTime;

        public Execute_unit (Node node)
        {
            this.node = node;
        }

        
    }

    class FCFS_Exexute_unit: Execute_unit, IComparable<FCFS_Exexute_unit>
    {
        public FCFS_Exexute_unit(Node node) : base(node) { }
        public int CompareTo(FCFS_Exexute_unit other)
        {
            if (this.node.arrive_time < other.node.arrive_time) return -1;
            else if (this.node.arrive_time == other.node.arrive_time) return 0;
            else return 1;
        }
    }

    class HRN_Exexute_unit : Execute_unit, IComparable<HRN_Exexute_unit>
    {
        int nowTime;
        public HRN_Exexute_unit(Node node) : base(node) { }
        public int CompareTo(HRN_Exexute_unit other)
        {
            if (this.node.arrive_time < other.node.arrive_time) return -1;
            else if (this.node.arrive_time == other.node.arrive_time) return 0;
            else return 1;
        }
    }

    class SJF_Exexute_unit : Execute_unit, IComparable<FCFS_Exexute_unit>
    {
        public SJF_Exexute_unit(Node node) : base(node) { }
        public int CompareTo(FCFS_Exexute_unit other)
        {
            if (this.node.service_time < other.node.service_time) return -1;
            else if (this.node.service_time == other.node.service_time) return 0;
            else return 1;
        }
    }

    class RR_Execute_unit : Execute_unit, IComparable<RR_Execute_unit>
    {
        public int need_time;
        public RR_Execute_unit(Node node) : base(node) { }

        public int CompareTo(RR_Execute_unit other)
        {
            if (this.node.arrive_time < other.node.arrive_time) return -1;
            else if (this.node.arrive_time == other.node.arrive_time) return 0;
            else return 1;
        }
    }
}

