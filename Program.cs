using System;

[Serializable]
public class ListNode
{
    public ListNode Prev;
    public ListNode Next;
    public ListNode Rand; // произвольный элемент внутри списка
    public string Data;
}

class Program
{
    static void Main()
    {
        ListRand listRand = new ListRand();
        listRand.Head.Data = "0";
        listRand.Tail.Data = "1";
        listRand.Add("2");
        listRand.Add("3");
        listRand.Add("4");
        listRand.Random();

        ListNode current = listRand.Head;
        for (int i= 0; i < listRand.Count; i++ )
        {
            Console.WriteLine("Значения в листе " + current.Data);
            current = current.Next;
        }
        listRand.Serialize();
        listRand.Deserialize();
    }

}
public class ListRand
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    public ListRand()
    {
        Head = new ListNode();
        Tail = new ListNode();
        Head.Next = Tail;
        Tail.Prev = Head;
        Count = 2;
    } 
        public void Add(string data)
    {
        var item = new ListNode();
        item.Data = data;
        Tail.Next = item;
        item.Prev = Tail;
        Tail = item;
        Count++;
    }
        public void Random()
    {
        //связь как в условии
        Head.Next.Rand = Head.Next;
        Head.Next.Next.Rand = Tail;
        Tail.Prev.Rand = Head;
    }
    public void Serialize()
    {
        int hashCode = 0; 
        int index = 0;
        Dictionary<ListNode, int> dict = new Dictionary<ListNode, int>();
        ListNode current = this.Head;
        byte[] byteArray = new byte[4];
        byte[] zeroArray = new byte[4];
        byte[] verifyArray = new byte[4];

        while (current != null)
        {
            dict.Add(current, index);
            current = current.Next;
            index++;
        }

        if(File.Exists(@"C:\Users\Panda\hwapp\save"))    //s.name))
        {
            using (var s = File.Open("save", FileMode.Open))
            {
                using (var reader = new BinaryReader(s))
                {
                    reader.Read(verifyArray, 0, byteArray.Length);
                }
            }
        }

            using (var s = File.Open("save", FileMode.Open))
            {
                hashCode = s.GetHashCode();
                byteArray = BitConverter.GetBytes(hashCode);
                Console.WriteLine(BitConverter.ToString(byteArray));
                if(!Enumerable.SequenceEqual(byteArray, verifyArray) || Enumerable.SequenceEqual(verifyArray, zeroArray))
                {
                    using (BinaryWriter writer = new BinaryWriter(s))
                    { 
                        writer.Write(byteArray,0,byteArray.Length);
                        for (ListNode cur = Head; cur != null; cur = cur.Next)
                        {
                            writer.Write(cur.Data);
                            if (cur.Rand != null) writer.Write(dict[cur.Rand]);
                            else writer.Write(-1);
                        }
                    }   
                }
            }
        }
    
    public void Deserialize()
    {
        int readCount = 0;
        Dictionary<int, string> readData = new Dictionary<int, string>();
        Dictionary<int, int> randomInt = new Dictionary<int, int>();

        using (var s = File.Open("save", FileMode.Open))
        {       
            using (var reader = new BinaryReader(s))
            {
                reader.BaseStream.Position = 4;
                while (reader.PeekChar() != -1)
                {
                    string data = reader.ReadString();
                    readData.Add(readCount, data);

                    int rand = reader.ReadInt32();
                    randomInt.Add(readCount, rand);

                    readCount++;
                }
            }
        }

        //создание двусвязного списка без ссылок на rand-элементы
        Count = readCount;
        Head = new ListNode();
        ListNode current = Head;
        Dictionary<int, ListNode> randomNode = new Dictionary<int, ListNode>();
        for (int i = 0; i < Count; i++)
        {
            randomNode.Add(i, current);
            current.Data = readData[i];
            if (i != Count - 1)
            {
                current.Next = new ListNode();
                current.Next.Prev = current;
                current = current.Next;
            }
            else Tail = current;
        }

        //добавление ссылок на rand-элементы
        current = Head;
        for (int i = 0; i < Count; i++)
        {
            if (randomInt[i] != -1) current.Rand = randomNode[randomInt[i]];
            current=current.Next;
        }

        //проверка
        int index = 0;
        for (current = Head; current != null; current = current.Next)
        {   
            if (current.Rand != null)
            {
            Console.WriteLine("Ячейка номер " + index);
            Console.WriteLine("Дата в рандоме " + current.Rand.Data);
            index++;
            } 
            else
            {
            Console.WriteLine("Ячейка номер " + index);
            Console.WriteLine("Дата в рандоме null");
            index++;
            }
        }
    }
}