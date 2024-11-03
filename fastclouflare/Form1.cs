using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Principal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace fastclouflare
{
    public partial class fastcf : Form
    {
        bool check=false;
        private Process cmdProcess;

        public fastcf()
        {
            InitializeComponent();
            if (!IsRunningAsAdministrator())
            {
                // 重新启动程序并赋予管理员权限
                StartAsAdministrator();
                Environment.Exit(0);
            }
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 已做为管理员启动" + Environment.NewLine);
            this.Size = new System.Drawing.Size(387, 387);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.cffalse;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (check==false)
            {
                mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 优选程序已启动" + Environment.NewLine);
                //strat
                check = true;
                pictureBox1.Image = Properties.Resources.cftrue;

                cmdProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                cmdProcess.OutputDataReceived += CmdProcess_OutputDataReceived;
                cmdProcess.ErrorDataReceived += CmdProcess_ErrorDataReceived;
                cmdProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                cmdProcess.EnableRaisingEvents = true;
                StartCmdProcess();
            }
            else
            {
                mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 优选程序已关闭" + Environment.NewLine);
                check = false;
                pictureBox1.Image = Properties.Resources.cffalse;
                // 检查进程是否已经启动且尚未退出
                if (cmdProcess != null && !cmdProcess.HasExited)
                {
                    try
                    {
                        // 尝试正常结束进程
                        cmdProcess.CloseMainWindow();
                        // 等待进程正常退出，设置超时时间
                        cmdProcess.WaitForExit(1000); // 等待1秒
                        if (!cmdProcess.HasExited)
                        {
                            // 如果进程没有正常退出，则强制结束进程
                            cmdProcess.Kill();
                            Console.WriteLine("cmd进程被锁定无法关闭");
                        }
                        else
                        {
                            Console.WriteLine("cmd程序已退出");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"退出cmd程序时遇到了问题： {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("cmd进程没运行");
                }
            }
        }
        private void StartCmdProcess()
        {
            if (!File.Exists("CloudflareST.exe"))
            {
                richTextBox1.AppendText("CloudflareST.exe 未找到，请重新下载zip" + Environment.NewLine);
                return;
            }

            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();

            // Run CloudflareST.exe and redirect output to newcfip.txt
            cmdProcess.StandardInput.WriteLine("CloudflareST.exe -o newcfip.txt");
            cmdProcess.StandardInput.Flush();
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 已启动CloudflareST.exe" + Environment.NewLine);
        }
        private void CmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // 原有的输出处理逻辑
            richTextBox1.Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText(e.Data + Environment.NewLine);
            });
            // 检查输出中是否包含退出指令的关键字
            if (!string.IsNullOrEmpty(e.Data) && (e.Data.Contains("按下回车键") || e.Data.Contains("Ctrl+C退出")|| e.Data.Contains("完整测速结果已写入")))
            {
                mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： CloudflareST.exe已退出" + Environment.NewLine);
                changehosts();
                // 如果找到关键字，停止接收数据并关闭CMD进程
                cmdProcess.CancelOutputRead();
                cmdProcess.Close();
                cmdProcess.Dispose();
            }

            
        }

        private void CmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // This method is called when the CMD process outputs error data
            richTextBox1.Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText(System.DateTime.Now.ToString("F") + "Error:" +e.Data + Environment.NewLine);
            });
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text== "↓ 展开查看日志")
            {
                mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 详细信息已展示" + Environment.NewLine);
                this.Size = new System.Drawing.Size(387, 598);
                button1.Text = "↑ 收起";
            }
            else
            {
                mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 详细信息已收起" + Environment.NewLine);
                this.Size = new System.Drawing.Size(387, 387);
                button1.Text = "↓ 展开查看日志";
            }
            
        }


        private void changehosts()
        {
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： 优选完成，开始修改hosts" + Environment.NewLine);
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string newCfIpFilePath = Path.Combine(currentDirectory, "newcfip.txt");
            string hostsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "drivers", "etc", "hosts");

            // 读取newcfip.txt文件中的第一个IP地址
            string firstIp = GetFirstIpFromNewCfIpFile(newCfIpFilePath);
            if (string.IsNullOrEmpty(firstIp))
            {
                MessageBox.Show("无法从newcfip.txt文件中读取IP地址。请重新执行程序");
                return;
            }
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + $"： 获取的第一个IP地址是：{firstIp}" + Environment.NewLine);

            // 备份hosts文件
            BackupHostsFile(hostsFilePath);
            
            // 编辑hosts文件
            EditHostsFile(hostsFilePath, firstIp);
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： hosts已编辑完成" + Environment.NewLine);
            ClearDnsCache();
            mainrizhi.AppendText(System.DateTime.Now.ToString("F") + "： dns缓存已清空" + Environment.NewLine);
            MessageBox.Show("优选完成，请愉快的浏览关于cf网站吧");
        }
        static string GetFirstIpFromNewCfIpFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length > 1)
                {
                    string firstLine = lines[1]; // 跳过标题行
                    string[] parts = firstLine.Split(',');
                    return parts[0]; // IP地址在第一列
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取文件时出错：{ex.Message}");
            }
            return null;
        }
        static void BackupHostsFile(string hostsFilePath)
        {
            try
            {
                string backupPath = hostsFilePath + "_backup";
                File.Copy(hostsFilePath, backupPath, true);
                Console.WriteLine($"hosts文件已备份到：{backupPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"备份hosts文件时出错：{ex.Message}");
            }
        }


        static void EditHostsFile(string hostsFilePath, string newIp)
        {
            try
            {
                string hostsContent = File.ReadAllText(hostsFilePath);
                bool containsDash = hostsContent.Contains("dash.cloudflare.com");
                bool containsChallenges = hostsContent.Contains("challenges.cloudflare.com");

                // 如果不存在 dash.cloudflare.com，则添加
                if (!containsDash)
                {
                    hostsContent = newIp + " dash.cloudflare.com"+"\n" + hostsContent;
                }
                // 如果不存在 challenges.cloudflare.com，则添加
                if (!containsChallenges)
                {
                    hostsContent = newIp + " challenges.cloudflare.com"+ "\n"+ hostsContent;
                }

                
                // 更新已存在的 IP 地址
                string updatedContent = Regex.Replace(hostsContent, @"(?<=\n|^)(challenges\.cloudflare\.com)\s+\d+\.\d+\.\d+\.\d+", newIp + " $1");
                updatedContent = Regex.Replace(updatedContent, @"(?<=\n|^)(dash\.cloudflare\.com)\s+\d+\.\d+\.\d+\.\d+", newIp + " $1");

                File.WriteAllText(hostsFilePath, updatedContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"编辑hosts文件时出错：{ex.Message}");
            }
        }
        static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        static void StartAsAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas";

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show("此程序需要管理员权限才能运行。请以管理员身份启动程序。", "权限错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void ClearDnsCache()
        {
            try
            {
                // 创建一个新的进程启动配置
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // 使用cmd.exe执行命令
                    Arguments = "/c ipconfig /flushdns", // 清理DNS缓存的命令
                    UseShellExecute = false, // 不使用外壳程序启动进程
                    CreateNoWindow = true, // 不创建新窗口
                    RedirectStandardOutput = true, // 重定向标准输出
                    RedirectStandardError = true // 重定向标准错误输出
                };

                // 创建进程
                using (Process process = new Process { StartInfo = startInfo })
                {
                    // 启动进程
                    process.Start();

                    // 读取标准输出
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // 等待进程结束
                    process.WaitForExit();

                    // 打印输出和错误信息
                    Console.WriteLine("Output: " + output);
                    Console.WriteLine("Error: " + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


    }
}
