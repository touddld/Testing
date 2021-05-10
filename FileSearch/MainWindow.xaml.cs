using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSearch
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Model
        {
            public string FileName { get; set; }
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }

        private ObservableCollection<Model> filelist = new ObservableCollection<Model>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            else
            {
                FileName.Text = m_Dialog.SelectedPath;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            filelist.Clear();
            List<string> Wordlist = new List<string>();
            string str = KeyWords.Text;
            string[] sArray = str.Split(' ');
            foreach (string i in sArray)
            {
                Wordlist.Add(i);
            }

            List<Model> Templist = new List<Model>();
            Director(FileName.Text, Templist);
            foreach(Model model in Templist)
            {
                if(CompareFile(model.FileName, Wordlist))
                {
                    filelist.Add(model);
                }
            }
            FilelistBox.ItemsSource = filelist;         
        }

        //文件检索生成文件列表filelist
        private void Director(string dir, List<Model> list)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileInfo[] files = d.GetFiles();//文件
            DirectoryInfo[] directs = d.GetDirectories();//文件夹
            foreach (FileInfo f in files)
            {
                list.Add(new Model
                {
                    Name = f.FullName,
                    FileName=f.Name,
                    IsSelected=true//添加文件名到列表中  
                });
            }
            //获取子文件夹内的文件列表，递归遍历  
            foreach (DirectoryInfo dd in directs)
            {
                Director(dd.FullName, list);
            }
        }

        //关键字匹配
        private bool CompareFile(string fileName,List<string> lst)
        {
            foreach(string s in lst)
            {
                if(!fileName.Contains(s.Trim()))
                {
                    return false;
                }
            }
            return true;
        }

        private void Ensure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog m_Dialog1 = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = m_Dialog1.ShowDialog();


                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    string FilePath = m_Dialog1.SelectedPath;
                    foreach (Model model in filelist)
                    {
                        if (model.IsSelected)
                        {
                            File.Copy(model.Name, FilePath + "\\" + model.FileName, true);
                        }
                    }
                }
                MessageBox.Show("保存成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存失败"+ex.Message);
            }
        }
    }
}
