using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace KararAğacı
{
    class Histogram
    {
        public string isim;
        public int sayı;
        public Histogram(string i,int s)
        {
            isim = i;
            sayı = s;
        }
        public static bool ListedeVarsaEkle(List<Histogram> Liste, string isim)
        {
            foreach (var item in Liste)
            {
                if (item.isim == isim)
                {
                    item.sayı += 1;
                    return true;
                }
            }
            Liste.Add(new Histogram(isim,1));
            return false;
        }
    }

    class GAIN
    {
        public string isim;
        public double değer;
        public List<Histogram> histo;
        public GAIN(string i,double d)
        {
            isim = i;
            değer = d;
        }
        public GAIN(string i, double d,List<Histogram> hlist)
        {
            isim = i;
            değer = d;
            histo = hlist;
        }
        public static GAIN EnBüyük(List<GAIN> gainList)
        {
            GAIN g=gainList[0];
            foreach (var item in gainList)
            {
                if (g.değer < item.değer)
                    g = item;
            }
            return g;
        }
    }

    class Program
    {

        public const char sep = ';';
        static void Main(string[] args)
        {
            
            string[] file = File.ReadAllLines("C:\\Users\\Eren\\Desktop\\KararAğacı\\Örnek.csv");

            Ağaç ağaç = new Ağaç();

            AğacıDoldur(ağaç, file, "Decision");

            AğacıYaz(ağaç);

            ağacıDosyayaYaz(ağaç);

            Console.ReadLine();
        }

        public static void AğacıDoldur(Ağaç ağaç,string[] dizi,string cevapSütun)
        {
            string[] sonuçlar = new string[dizi.Length - 1];
            int konum = Array.IndexOf(dizi[0].Split(sep), cevapSütun);

            for (int i = 0; i < dizi.Length - 1; i++)
            {
                sonuçlar[i] = dizi[i + 1].Split(sep)[konum];
            }
            double EntropiG = Entropi(sonuçlar);

            if (EntropiG == 0)
            {
                ağaç.sınıf = dizi[1].Split(sep)[konum];
            }
            else
            {
                List<GAIN> GainList = Gain(dizi,cevapSütun);
                GAIN Enbüyük = GAIN.EnBüyük(GainList);
                ağaç.isim = Enbüyük.isim;

                foreach (var dal in Enbüyük.histo)
                {
                    Ağaç AltAğaç = new Ağaç();
                    AltAğaç.değer = dal.isim;

                    string[] AltDizi = altDizi(dizi, dal, Enbüyük.isim);

                    ağaç.AltListe.Add(AltAğaç);

                    AğacıDoldur(AltAğaç, AltDizi, cevapSütun);
                }

            }

        }
        
        public static void AğacıYaz(Ağaç ağaç,string boşluk="")
        {
            if (ağaç.AltListe.Count!=0)
            {
                foreach (var dal in ağaç.AltListe)
                {
                    Console.WriteLine(boşluk + ağaç.isim + ":" + dal.değer);
                    AğacıYaz(dal, boşluk + "    ");
                }
                
            }
            else
            {
                Console.WriteLine(boşluk + ağaç.sınıf);
            }
        }

        public static void ağacıDosyayaYaz(Ağaç ağaç, string dosya = "ağaçdosyası.txt",string boşluk="")
        {
            if (ağaç.AltListe.Count != 0)
            {
                foreach (var dal in ağaç.AltListe)
                {
                    File.AppendAllText(dosya,boşluk + ağaç.isim + ":" + dal.değer+"\r\n");
                    ağacıDosyayaYaz(dal,dosya, boşluk + "    ");
                }

            }
            else
            {
                File.AppendAllText(dosya,boşluk + ağaç.sınıf+"\r\n");
            }
        }

        public static List<GAIN> Gain(string[] dizi,string cevapSütun)
        {

            List<GAIN> GainList = new List<GAIN>();

            if (dizi == null || dizi.Length == 0 )
                return GainList;


            string[] sonuçlar = new string[dizi.Length - 1];
            int konum = Array.IndexOf(dizi[0].Split(sep), cevapSütun);

            for (int i = 0; i < dizi.Length - 1; i++)
            {
                sonuçlar[i] = dizi[i + 1].Split(sep)[konum];
            }
            double EntropiG = Entropi(sonuçlar);
            //Console.WriteLine("Genel Entropi " + EntropiG.ToString());

            foreach (var item in dizi[0].Split(sep))
            {
                if (item == cevapSütun) continue;
                konum=Array.IndexOf(dizi[0].Split(sep), item);

                string[] sütunlar = new string[dizi.Length - 1];

                for (int i = 0; i < dizi.Length - 1; i++)
                {
                    sütunlar[i] = dizi[i + 1].Split(sep)[konum];
                }

                List<Histogram> histo = new List<Histogram>();
                histo.Add(new Histogram(sütunlar[0], 0));

                for (int i = 0; i < sütunlar.Length; i++)
                {
                    Histogram.ListedeVarsaEkle(histo, sütunlar[i]);
                }

                double sütunEntropi = 0.0;
                foreach (var i in histo)
                {
                    string[] usütun = DizidenSec(dizi, i, item, cevapSütun);
                    double entropix = Entropi(usütun);
                    sütunEntropi += ((double)i.sayı/sütunlar.Length) * entropix;
                    //Console.WriteLine(i.sayı + " " + sütunlar.Length + " " + entropix);
                }
                GainList.Add(new GAIN(item,EntropiG - sütunEntropi,histo));
            }

            //Console.WriteLine(EntropiG.ToString());
            //Console.ReadLine();
            return GainList;
        }

        public static string[] DizidenSec(string[] dizi,Histogram histo,string sütuni,string sütuns)
        {
            int konum = Array.IndexOf(dizi[0].Split(sep), sütuni);
            int konums = Array.IndexOf(dizi[0].Split(sep), sütuns);
            string[] sütunlar = new string[histo.sayı];
            int count=0;

            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].Split(sep)[konum] == histo.isim)
                {
                    sütunlar[count] = dizi[i].Split(sep)[konums];
                    count += 1;
                }
                
            }
            return sütunlar;
        }
        public static string[] altDizi(string[] dizi, Histogram histo,string sütunisim)
        {
            string[] sütunlar = new string[histo.sayı+1];

            int konum = Array.IndexOf(dizi[0].Split(sep), sütunisim);
            sütunlar[0] = dizi[0].Replace(dizi[0].Split(sep)[konum] + sep, "").Replace(sep + dizi[0].Split(sep)[konum], "");
            int count = 1;
            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].Split(sep)[konum] == histo.isim)
                {
                    sütunlar[count] = dizi[i].Replace(dizi[i].Split(sep)[konum]+sep, "").Replace(sep+dizi[i].Split(sep)[konum], "");
                    count += 1;
                }

            }

            return sütunlar;
        }
        public static double Entropi(string[] dizi)
        {
            double EntropiDeğeri = 0.0;
            List<Histogram> histo = new List<Histogram>();
            histo.Add(new Histogram(dizi[0], 0));


            for (int i = 0; i < dizi.Length; i++)
            {
                Histogram.ListedeVarsaEkle(histo, dizi[i]);
            }
            

            foreach (var item in histo)
            {
                //Console.WriteLine(item.isim + " " + item.sayı);
                double olasılık = (double)item.sayı / (double)dizi.Length;
                EntropiDeğeri += -(double)(olasılık) * (Math.Log(olasılık) / Math.Log(2));
            }

            return EntropiDeğeri;
        }
        
    }
}
