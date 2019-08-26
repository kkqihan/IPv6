// how to check IPv6 network before submit to AppStore
// https://developer.apple.com/library/content/documentation/NetworkingInternetWeb/Conceptual/NetworkingOverview/UnderstandingandPreparingfortheIPv6Transition/UnderstandingandPreparingfortheIPv6Transition.html#//apple_ref/doc/uid/TP40010220-CH213-SW1

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class DemoTest : MonoBehaviour {
	void Start () {
		string outStr;
		IPv6Tool.Instance.TryConvertIPv4ToIPv6ForWWW ("http://39.106.210.67:80", out outStr);
		IPv6Tool.Instance.TryConvertIPv4ToIPv6ForTCP ("39.106.210.67", out outStr);
	}

}