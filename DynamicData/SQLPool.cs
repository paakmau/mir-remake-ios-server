using System;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
namespace MirRemakeBackend.DynamicData
{
    class SQLPool
    {
        private class ConnectionPool
        {
            /// <summary>
            /// 链接字符串
            /// </summary>
            private readonly string connstr;

            /// <summary>
            /// 缓存链接数量
            /// </summary>
            private readonly int itemCount;

            /// <summary>
            /// 所有的链接
            /// </summary>
            private readonly ConcurrentQueue<SqlConnection> conns = new ConcurrentQueue<SqlConnection>();

            public ConnectionPool(SqlConfig config)
            {
                this.itemCount = config.cacheCount;
                this.connstr = string.Format("Data Source={0},{1};user id={2};pwd={3};initial catalog=",
                                             config.host, config.port, config.username, config.pwd);
            }

            /// <summary>
            /// 获取一个连接
            /// </summary>
            /// <param name="dbbase"></param>
            /// <returns></returns>
            public SqlConnection GetConnection(string dbbase)
            {
                SqlConnection conn;
                if (conns.TryDequeue(out conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    try
                    {
                        conn.ChangeDatabase(dbbase);
                        //这里有可能被数据库主动关闭,客户端根本不知道，执行将会报错！不应该存储链接，用完即放入连接池，不然应用的时候也可能会报错，被服务器主动断开
                    }
                    catch
                    {
                        conn = null;
                    }
                    if (conn != null)
                        return conn;
                }
                Console.WriteLine("创建一链接!" + conns.Count);
                return new SqlConnection(this.connstr + dbbase);
            }

            /// <summary>
            /// 放入一个连接
            /// </summary>
            /// <param name="conn"></param>
            public void Push(SqlConnection conn)
            {
                Task.Factory.StartNew(
                        () => {
                            if (conns.Count < itemCount)
                            {
                                //Console.WriteLine("放入一个连接Begin");
                                conns.Enqueue(conn); //好像不会阻塞,Task可能是多余的,
                                //Console.WriteLine("放入一个链接!");
                            }
                            else
                                conn.Dispose();
                        }
                    );
            }
        }
        private ConnectionPool ConnPool = null;
        public SQLPool(SqlConfig config)
        {
            this.ConnPool = new ConnectionPool(config);
        }
        public void ExecuteSql(string dbbase, string sql, DataSet ds = null)
        {
            SqlCommand cmd = new SqlCommand(sql, ConnPool.GetConnection(dbbase));
            cmd.CommandType = CommandType.Text;
            Execute(cmd, ds);
        }
        private void Execute(SqlCommand cmd, DataSet ds = null)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            if (ds == null)
                cmd.ExecuteNonQuery();
            else
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(ds);   //调用会自动执行 ExecuteNonQuery();    不要重复调用,很容易出错!
                sda.Dispose();  //一定要关闭,不然下次执行报错;
            }
            ConnPool.Push(cmd.Connection);   //放入链接
        }
    }
    class SqlConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public IPAddress host;

        /// <summary>
        /// 端口
        /// </summary>
        public int port;

        /// <summary>
        /// 用户名
        /// </summary>
        public string username;

        /// <summary>
        /// 密码
        /// </summary>
        public string pwd;

        /// <summary>
        /// 缓存链接数量
        /// </summary>
        public int cacheCount = 100;
    }
    public class SqlProdureParameter
    {
        /*
            public SqlCommand sqlcommand {
                get;
                private set;
            }
    */
        public SqlCommand sqlcommand
        {
            get;
            private set;
        }

        private SqlProdureParameter() { }

        internal SqlProdureParameter(SqlCommand sqlcommand)
        {
            this.sqlcommand = sqlcommand;
        }
        /*
                internal SqlProdureParameter(MySqlCommand mysqlcommand) {
                    this.mysqlcommand = mysqlcommand;
                }
        */
        /// <summary>
        /// 释放
        /// </summary>
        internal void Dispose()
        {
            sqlcommand = null;
        }
    }
}
