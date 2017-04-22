using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ClientLib.Entities
{
    public class DownloadState : INotifyPropertyChanged
    {
        private int _bytesRead;
        /// <summary>
        /// 当前读取的字节数
        /// </summary>
        public int BytesRead
        {
            get { return _bytesRead; }
            set
            {
                if (_bytesRead != value)
                {
                    _bytesRead = value;///  asdfasdf    adsfadsf
                    OnPropertyChanged("BytesRead");
                }
            }
        }

        private long _totalBytes;
        /// <summary>
        /// 文件的大小
        /// </summary>
        public long TotalBytes
        {
            get { return _totalBytes; }
            set
            {
                if (_totalBytes != value)
                {
                    _totalBytes = value;
                    OnPropertyChanged("TotalBytes");
                }

            }
        }

        private double _downloadPercent;
        /// <summary>
        /// 下载百分比
        /// </summary>
        public double DownloadPercent
        {
            get { return _downloadPercent; }
            set
            {
                if (_downloadPercent != value)
                {
                    _downloadPercent = value;
                    OnPropertyChanged("DownloadPercent");
                }
            }
        }

        private bool _isDownloading = false;
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                if (value != _isDownloading)
                {
                    _isDownloading = value;
                    OnPropertyChanged("IsDownloading");
                }
            }
        }

        public CancellationTokenSource CancellationToken { get; private set; }

        public DownloadState()
        {
            this.CancellationToken = new CancellationTokenSource();
        }

        public void CancelDownload()
        {
      

            //this.CancellationToken.Cancel();
            //this.CancellationToken = null;
            //this.CancellationToken = new CancellationTokenSource();
        }

        public void Reset()
        {
            this.BytesRead = 0;
            this.TotalBytes = 0;
            this.DownloadPercent = 0;
            this.IsDownloading = false;

            this.CancellationToken.Dispose();
            this.CancellationToken = new CancellationTokenSource();
        }

        public void Set()
        {
            this.BytesRead = 0;
            this.TotalBytes = 0;
            this.DownloadPercent = 0;
            this.IsDownloading = true;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class UploadState : INotifyPropertyChanged
    {
        private int _bytesWrite;

        public int BytesWrite
        {
            get { return _bytesWrite; }
            set
            {
                if (value != _bytesWrite)
                {
                    _bytesWrite = value;
                    OnPropertyChanged("BytesWrite");
                }
            }
        }

        private long _totalBytes;

        public long TotalBytes
        {
            get { return _totalBytes; }
            set
            {
                if (value != _totalBytes)
                {
                    _totalBytes = value;
                    OnPropertyChanged("TotalBytes");
                }
            }
        }

        private double _uploadPercent;

        public double UploadPercent
        {
            get { return _uploadPercent; }
            set
            {
                if (value != _uploadPercent)
                {
                    _uploadPercent = value;
                    OnPropertyChanged("UploadPercent");
                }
            }
        }

        public CancellationTokenSource CancellationToken { get; private set; }

        public UploadState()
        {
            this.CancellationToken = new CancellationTokenSource();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
