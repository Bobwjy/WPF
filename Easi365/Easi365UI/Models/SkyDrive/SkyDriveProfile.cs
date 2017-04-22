using ClientLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easi365UI.Models.SkyDrive
{
    public class SkyDriveProfile : IProfile
    {
        //处理Task中的异常信息
        public EventHandler<AggregateExceptionArgs> AggregateExceptionCatched;

        public class AggregateExceptionArgs : EventArgs
        {
            public AggregateException AggregateException { get; set; }
        }

        /// <summary>
        /// 服务器端同步类
        /// </summary>
        SyncManager _sm;
        CancellationTokenSource _cts = null; //取消操作的标记
        SynchronizationContext _sc; //UI线程的上下文

        ServerSide _server;

        //public SkyDriveProfile(SyncManager sm, SynchronizationContext sc, CancellationTokenSource cts)
        //{
        //    this._sm = sm;
        //    this._sc = sc;
        //    this._cts = cts;
        //    //this.ModelCache = new SkyDriveModelCache();
        //}

        public SkyDriveProfile(ServerSide server, SynchronizationContext sc, CancellationTokenSource cts)
        {
            this._server = server;
            this._sc = sc;
            this._cts = cts;
            //this.ModelCache = new SkyDriveModelCache();
        }

        public Task<IEntryModel> ParseAsync(string path)
        {
            throw new NotImplementedException();
        }

       

        public Task ListAsync(IEntryModel entry, ObservableCollection<IEntryModel> fileListItems, bool refresh = false)
        {
            return Task.Factory.StartNew(delegate()
            {
                try
                {
                    //var folderItems = _sm.PersonalServer.GetItemsInFolder(entry.FullPath, 0);
                    var folderItems = _server.GetItemsInFolder(entry.FullPath, 0);
                    //获取到的数据同步到UI线程中的集合
                    _sc.Post((state) =>
                    {
                        fileListItems.Clear();

                        folderItems.ForEach(c =>
                        {
                            var model = new SkyDriveItemModel(this, c);
                            //设置文件列表中的是否“本地缓存”的列
                            //_server.CheckFileIfCached(c, b => 
                            //{ 
                            //    if (b)
                            //        model.IsLocalCached = "本地缓存"; 
                            //});
                            fileListItems.Add(model);
                        });
                    }, null);
                }
                catch (Exception ex)
                {
                    AggregateExceptionArgs errArgs = new AggregateExceptionArgs();
                    errArgs.AggregateException = new AggregateException(ex);

                    if (AggregateExceptionCatched != null)
                        AggregateExceptionCatched(null, errArgs);
                }
            }, _cts.Token);
        }

        public Task ListSearchAsync(string content, ObservableCollection<IEntryModel> fileListItems, IEntryModel entry, bool refresh = false)
        {
            return Task.Factory.StartNew(delegate()
            {
                try
                {
                    //var folderItems = _sm.PersonalServer.GetItemsInFolder(entry.FullPath, 0);
                    var folderItems = _server.SearchItems(content, 0, entry.FullPath);
                    //获取到的数据同步到UI线程中的集合
                    _sc.Post((state) =>
                    {
                        fileListItems.Clear();

                        folderItems.ForEach(c =>
                        {
                            var model = new SkyDriveItemModel(this, c);
                            //设置文件列表中的是否“本地缓存”的列
                            //_server.CheckFileIfCached(c, b => 
                            //{ 
                            //    if (b)
                            //        model.IsLocalCached = "本地缓存"; 
                            //});
                            fileListItems.Add(model);
                        });
                    }, null);
                }
                catch (Exception ex)
                {
                    AggregateExceptionArgs errArgs = new AggregateExceptionArgs();
                    errArgs.AggregateException = new AggregateException(ex);

                    if (AggregateExceptionCatched != null)
                        AggregateExceptionCatched(null, errArgs);
                }
            }, _cts.Token);
        }
        /// <summary>
        /// 关注的文档
        /// </summary>
        /// <param name="fileListItems"></param>
        //public void FollowListAsync(ObservableCollection<IEntryModel> followListItems)
        //{
        //    Task.Factory.StartNew(delegate()
        //    {
        //        try
        //        {
        //            var followItems = (_server as PersonalServerSide).GetFollowItems();

        //            _sc.Post((state) =>
        //            {
        //                followListItems.Clear();

        //                followItems.ForEach(c =>
        //                {
        //                    followListItems.Add(new SkyDriveItemModel(this)
        //                    {
        //                        Name = c.Name,
        //                        FullPath = c.ServerRelativeUrl,
        //                       Modified = c.Modified
        //                    });
        //                });
        //            }, null);
        //        }
        //        catch (Exception ex)
        //        {
        //            AggregateExceptionArgs errArgs = new AggregateExceptionArgs();
        //            errArgs.AggregateException = new AggregateException(ex);

        //            if (AggregateExceptionCatched != null)
        //                AggregateExceptionCatched(null, errArgs);
        //        }
        //    }, _cts.Token);
        //}

        /// <summary>
        /// 文档的关注与取消关注
        /// </summary>
        /// <param name="followListItems"></param>
        /// <param name="model"></param>
        /// <param name="isFollow">关注:true, 取消关注:false</param>
        /// <returns></returns>
        //public Task<bool> DocumentFollowingAsync(ObservableCollection<IEntryModel> followListItems, IEntryModel model, bool isFollow)
        //{
        //    return Task<bool>.Factory.StartNew(delegate()
        //    {
        //        try
        //        {
        //            (_server as PersonalServerSide).DocumentFollowing(model.FullPath, isFollow);

        //            _sc.Post((state) =>
        //            {
        //                if (isFollow)
        //                {
        //                    followListItems.Add(model);
        //                }
        //                else
        //                {
        //                    followListItems.Remove(model);
        //                }
        //            }, null);

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            AggregateExceptionArgs errArgs = new AggregateExceptionArgs();
        //            errArgs.AggregateException = new AggregateException(ex);

        //            if (AggregateExceptionCatched != null)
        //                AggregateExceptionCatched(null, errArgs);
        //        }

        //        return false;
        //    }, _cts.Token);
        //}

        /// <summary>
        /// 分页读取共享给我的文档
        /// </summary>
        /// <param name="sharedWithMeItems"></param>
        /// <returns></returns>
        //public Task<bool> GetDocumentsSharedWithMeAsync(ObservableCollection<IEntryModel> sharedWithMeItems)
        //{
        //    return Task<bool>.Factory.StartNew(delegate()
        //    {
        //        try
        //        {
        //            var items = (_server as PersonalServerSide).GetSharedWithMeListItem();
        //            if (items == null) return false;

        //            _sc.Post((state) =>
        //            {
        //                sharedWithMeItems.Clear();

        //                items.ForEach(m =>
        //                {
        //                    sharedWithMeItems.Add(new SkyDriveItemModel(this)
        //                    {
        //                        Name = m.FileLeafRef,
        //                        FullPath = m.Path,
        //                        IsDirectory = m.FSObjType == 1,
        //                        Modified = m.Modified,
        //                        ServerItem = m
        //                    });
        //                });
        //            }, null);

        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            AggregateExceptionArgs errArgs = new AggregateExceptionArgs();
        //            errArgs.AggregateException = new AggregateException(ex);

        //            if (AggregateExceptionCatched != null)
        //                AggregateExceptionCatched(null, errArgs);
        //        }

        //        return false;
        //    }, _cts.Token);
        //}


        /// <summary>
        /// 服务器端对象
        /// </summary>
        public ServerSide Server
        {
            get { return _server; }
        }
    }
}
