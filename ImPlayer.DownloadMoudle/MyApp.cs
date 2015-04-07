using MyDownloader.App;
using MyDownloader.Core.Extensions;
using MyDownloader.Core.UI;
using MyDownloader.Extension.PersistedList;
using MyDownloader.Extension.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle
{
    public class MyApp : IApp
    {

        #region IApp 成员

        #region Singleton

        private static MyApp instance = new MyApp();

        public static MyApp Instance
        {
            get
            {
                return instance;
            }
        }

        private MyApp()
        {
            AppManager.Instance.Initialize(this);

            extensions = new List<IExtension>();

            extensions.Add(new CoreExtention());
            extensions.Add(new HttpFtpProtocolExtension());
            extensions.Add(new PersistedListExtension());

        }

        #endregion

        #region Fields

        private List<IExtension> extensions;
        //  private SingleInstanceTracker tracker = null;
        private bool disposed = false;

        #endregion

        #region Properties



        public List<IExtension> Extensions
        {
            get
            {
                return extensions;
            }
        }

        #endregion

        #region Methods

        public IExtension GetExtensionByType(Type type)
        {
            for (int i = 0; i < this.extensions.Count; i++)
            {
                if (this.extensions[i].GetType() == type)
                {
                    return this.extensions[i];
                }
            }

            return null;
        }

        public void InitExtensions()
        {
            for (int i = 0; i < Extensions.Count; i++)
            {
                if (Extensions[i] is IInitializable)
                {
                    ((IInitializable)Extensions[i]).Init();
                }
            }

        }
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                for (int i = 0; i < Extensions.Count; i++)
                {
                    if (Extensions[i] is IDisposable)
                    {
                        try
                        {
                            ((IDisposable)Extensions[i]).Dispose();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
