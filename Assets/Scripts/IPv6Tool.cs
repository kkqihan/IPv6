using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

public class IPv6Tool
{
	#region Unity call IOS
	//#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern string getIPv6 (string mHost);
	//#endif
	#endregion

	#region 成员变量 静态
	private static List<string> allProtocolStrList = new List<string> () { "http://", "https://" };
	/// <summary>
	/// 静态单例
	/// 私有
	/// </summary>
	private static IPv6Tool instance = null;
	/// <summary>
	/// 静态单例
	/// 公开 用于外部获取
	/// </summary>
	public static IPv6Tool Instance {
		get {
			if (instance == null) {
				instance = new IPv6Tool ();
			}
			return instance;
		}
	}
	#endregion

	#region 成员变量 非静态

	#endregion

	#region 内部调用函数
	/// <summary>
	/// 尝试转换IPv4到IPv6
	/// 仅地址 不能包含端口、http协议头等
	/// </summary>
	/// <returns><c>true</c>转换成功 <c>false</c> 转换失败</returns>
	/// <param name="iPv4_str">输入的IPv4地址字符串</param>
	/// <param name="iPv6_str">输出的IPv6地址字符串</param>
	private bool tryConvertIPv4ToIPv6OnlyAddress (string iPv4_str, out string iPv6_str)
	{
		bool isSuccess = false;
		iPv6_str = string.Empty;

		string resultByIOS = getIPv6 (iPv4_str);
		if (!string.IsNullOrEmpty (resultByIOS)) { //IOS返回有可能为null
			string [] reslutParam = System.Text.RegularExpressions.Regex.Split (resultByIOS, "&&"); //将IOS返回值进行划分
			if (reslutParam != null && reslutParam.Length >= 2) {
				string iPType = reslutParam [1];
				if (iPType == "ipv6") {
					isSuccess = true;
					iPv6_str = reslutParam [0];
				}
			}
		}

		return isSuccess;
	}
	#endregion

	#region 外部调用函数
	/// <summary>
	/// 尝试转换IPv4到IPv6
	/// 专供www类的链接转换 必须为 协议头+IP+端口的形式 http://192.168.0.1:80
	/// </summary>
	/// <returns><c>true</c>成功转换为IPv6 <c>false</c> 转换失败，即当前网络为ipv4的网络</returns>
	/// <param name="iPv4_str">ipv4字符串</param>
	/// <param name="iPv6_str">ipv6字符串 返回</param>
	public bool TryConvertIPv4ToIPv6ForWWW (string iPv4_str, out string iPv6_str)
	{
		Debug.Log ("TryConvertIPv4ToIPv6ForWWW --->>iPv4_str:" + iPv4_str);

		bool isSuccess = false;
		iPv6_str = string.Empty;


#if UNITY_IPHONE && !UNITY_EDITOR

		//1.剪切协议头
		string cutProtocolStr = string.Empty;
		for (int idx = 0; idx < allProtocolStrList.Count; idx++) {
			string protocolStr = allProtocolStrList [0];
			if (iPv4_str.StartsWith (protocolStr)) {//如果以协议开头
				cutProtocolStr = protocolStr;//记录剪切的协议字符串
				iPv4_str = iPv4_str.Substring (protocolStr.Length);//ipv4字符串直接剪切掉协议头
				break;
			}
		}
		//2.剪切端口号
		string cutPort = string.Empty;
		if (iPv4_str.Contains (":")) {
			int splitIdx = iPv4_str.IndexOf (":");//记录分号位置
			cutPort = iPv4_str.Substring (splitIdx + 1);//分号后面全部为端口号
			iPv4_str = iPv4_str.Substring (0, splitIdx);//剪切尾部的 分号+端口号
		}
		//3.转换IPv6
		string convertAdd = string.Empty;
		if (tryConvertIPv4ToIPv6OnlyAddress (iPv4_str, out convertAdd)) {
			isSuccess = true;
		}
		//4.拼接协议头和端口号
		iPv6_str = string.Format ("{0}[{1}]:{2}", cutProtocolStr, convertAdd, cutPort);


#endif
		Debug.Log ("TryConvertIPv4ToIPv6ForWWW isSuccess=" + isSuccess + " <<---iPv6_str:" + iPv6_str);

		return isSuccess;
	}

	/// <summary>
	/// 尝试转换IPv4到IPv6
	/// 专供TCP链接类转换 必须为 纯IP形式 192.168.0.1
	/// </summary>
	/// <returns><c>true</c>成功转换为IPv6 <c>false</c> 转换失败，即当前网络为ipv4的网络</returns>
	/// <param name="iPv4_str">ipv4字符串</param>
	/// <param name="iPv6_str">ipv6字符串 返回</param>
	public bool TryConvertIPv4ToIPv6ForTCP (string iPv4_str, out string iPv6_str)
	{
		Debug.Log ("TryConvertIPv4ToIPv6ForTCP --->>iPv4_str:" + iPv4_str);

		bool isSuccess = false;
		iPv6_str = string.Empty;

#if UNITY_IPHONE && !UNITY_EDITOR

		//转换IPv6
		string convertAdd = string.Empty;
		if (tryConvertIPv4ToIPv6OnlyAddress (iPv4_str, out convertAdd)) {
			isSuccess = true;
		}
		//最后赋值
		iPv6_str = convertAdd;

#endif

		Debug.Log ("TryConvertIPv4ToIPv6ForTCP isSuccess=" + isSuccess + " <<---iPv6_str:" + iPv6_str);

		return isSuccess;

	}
	#endregion
}