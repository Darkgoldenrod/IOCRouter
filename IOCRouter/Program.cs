using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;


namespace IOCRouter
{
    public class Container
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Assembly assembly { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string nameSpace { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public List<MethodInfo> methodInfos { get; set; }
    }

    class Program
    {

        /// <summary>
        /// 容器
        /// </summary>
        public static List<Container> container = new List<Container>();


        static void Main(string[] args)
        {
            Init();

            //格式： （不区分大小写）
            //1. users/getalluserinfo
            //2. users/addnewuser?username=abc&phonenumber=123


            while (true)
            {
                string input = Console.ReadLine();
                if (input == "0") break;

                Console.WriteLine(Router(input));
            }


            //Console.WriteLine(Router("user/getalluserinfo"));
            //Console.ReadKey();

            ////添加用户
            //Console.WriteLine(Router("user/addnewuser?username=张三&phonenumber=123456"));
            //Console.WriteLine(Router("user/addnewuser?username=李四&phonenumber=777777"));
            //Console.WriteLine(Router("user/getalluserinfo"));
            //Console.ReadKey();

            ////删除用户
            //Console.WriteLine(Router("user/deleteuser?username=张三"));
            //Console.WriteLine(Router("user/getalluserinfo"));
            //Console.ReadKey();

            ////修改用户手机
            //Console.WriteLine(Router("user/modifyphonenumber?username=李四&phonenumber=999999"));
            //Console.WriteLine(Router("user/getalluserinfo"));
            //Console.ReadKey();




        }


        public static void Init()
        {
            try
            {
                var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "modules");
                var files = directory.GetFiles("*.dll");
                foreach (var f in files)
                {
                    var asm = Assembly.LoadFrom(f.FullName);
                    var methods = new List<MethodInfo>();

                    Type[] types = asm.GetTypes();
                    foreach (var t in types)
                    {                     
                        var ms = t.GetMethods();
                        foreach (var m in ms)
                        {
                            methods.Add(m);
                        }                    
                    }

                    string n = Path.GetFileNameWithoutExtension(f.FullName);
                    var cont = new Container()
                    {
                        className = Path.GetFileNameWithoutExtension(f.FullName),
                        assembly = asm,
                        nameSpace = n + "." + n,
                        methodInfos = methods
                    };

                    container.Add(cont);
                }
            }
            catch
            {
                Console.WriteLine("初始化失败！");
            }         
        }

        public static string Router(string address)
        {
            try
            {
                //1.拆
                string[] mark = address.Split('?');
                if (mark.Count() > 2)
                    return "请输入正确的路由地址！";

                string[] biases = mark[0].Split('/');
                if (biases.Count() != 2)
                    return "请输入正确的路由地址！";

                string className = biases[0];
                string function = biases[1];

                foreach (var c in container)
                {
                    if (c.className.ToLower() == className.ToLower())
                    {
                        foreach (var method in c.methodInfos)
                        {
                            if (method.Name.ToLower() == function.ToLower())
                            {
                                object o = Activator.CreateInstance(c.assembly.GetType(c.nameSpace));
                                var parameter = method.GetParameters();
                                

                                if (parameter == null || parameter.Count() == 0)
                                {
                                    //无参数
                                    //var keyData = method.Invoke(o, null);
                                    return method.Invoke(o, null).ToString();
                                }
                                else
                                {
                                    //解析参数
                                    List<string> and = mark[1].Split('&').ToList();
                                    List<string> par = new List<string>();                                                                    
                                    foreach (var p in parameter)
                                    {
                                        string vl = string.Empty;
                                        foreach (var a in and)
                                        {
                                            var equal = a.Split('=');
                                            if (p.Name.ToLower() == equal[0].ToLower())
                                                vl = equal[1];
                                        }
                                        if (string.IsNullOrEmpty(vl)) continue;
                                        par.Add(vl);
                                    }
                                    //var keyData = method.Invoke(o, par.ToArray());
                                    return method.Invoke(o, par.ToArray()).ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                
            }

            return "请输入正确的路由地址！";
        }



    }
}
