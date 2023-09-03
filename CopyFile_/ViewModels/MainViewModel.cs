using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CopyFile_.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Thread _thread { get; set; }
        private long progressbarLength = 0;
        public long ProgressBarLength
        {
            get { return progressbarLength; }
            set { progressbarLength = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private long progressbarMaxLength = 0;
        public long ProgressBarMaxLength
        {
            get { return progressbarMaxLength; }
            set { progressbarMaxLength = value; OnPropertyChanged(); }
        }

        private string senderFilePath;

        public string SenderFilePath
        {
            get { return senderFilePath; }
            set { SenderFilePath = value; OnPropertyChanged(); }
        }

        private string receiveFilePath;

        public string ReceiverFilePath
        {
            get { return receiveFilePath; }
            set { ReceiverFilePath = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _thread = new Thread(CopyFile);
        }

        public void CopyFile()
        {
            if (!File.Exists(SenderFilePath))
            {
                MessageBox.Show("Something went wrong...", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(ReceiverFilePath))
            {
                MessageBox.Show("Something went wrong...", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(ReceiverFilePath))
            {
                MessageBox.Show("Something went wrong...", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var filestream = new FileStream(SenderFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var filestream2 = new FileStream(ReceiverFilePath, FileMode.Open, FileAccess.Write))
                {
                    filestream2.Seek(filestream2.Length, SeekOrigin.Current);
                    ProgressBarMaxLength = filestream.Length;
                    byte lengthArray = 10;
                    byte[] byteArray = null;
                    for (long i = 0; i < ProgressBarMaxLength; i += lengthArray)
                    {
                        byteArray = new byte[lengthArray];
                        filestream2.Write(byteArray, 0, lengthArray);
                        ProgressBarLength += lengthArray;
                        filestream.Read(byteArray, 0, lengthArray);
                        Thread.Sleep(2);

                    }
                    MessageBox.Show("Succesfully Done!!!" +
                        "", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    _thread = new Thread(() =>
                    {
                        CopyFile();
                    });
                    SenderFilePath = null;
                    ReceiverFilePath = null;
                    ProgressBarLength = default;
                    ProgressBarMaxLength = 100;
                }
            }
        }


        public RelayCommand SendCommand
        {
            get => new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    SenderFilePath = openFileDialog.FileName;
                }

            });
        }

        public RelayCommand ReceiveCommand
        {
            get => new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    ReceiverFilePath = openFileDialog.FileName;
                }
            });
        }

        public RelayCommand CopyCommand
        {
            get => new RelayCommand(() =>
            {
                if (_thread.ThreadState != ThreadState.Running)
                {
                    _thread?.Start();

                }
                else MessageBox.Show("Upss...Something went wrong!!!!");

            });
        }


        public RelayCommand ResumeCommand
        {
            get => new RelayCommand(() =>
            {
                if (_thread.ThreadState == ThreadState.Suspended)
                {
                    _thread?.Resume();
                }
            });
        }

        public RelayCommand AbortCommand
        {
            get => new RelayCommand(() =>
            {
                if (_thread.ThreadState == ThreadState.Suspended)
                {
                    _thread.Resume();

                }
                _thread?.Abort();
                _thread = new Thread(() =>
                {
                    CopyFile();
                });

                SenderFilePath = null;
                ReceiverFilePath = null;
                ProgressBarLength = default;
                ProgressBarMaxLength = 100;

            });
        }

        public RelayCommand SuspendCommand
        {
            get => new RelayCommand(() =>
            {
                _thread.Suspend();
            });
        }
    }
}
