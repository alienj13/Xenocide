using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAccountDetails : MonoBehaviour
{
    [Header("Properties (Read-only)")]
    [SerializeField] private string SF_Username = "";
    [SerializeField] private string SF_UserRank = "";
    [SerializeField] private string SF_UserEXP = "";

    public static string username = "";
    public static string userRank = "";
    public static string userEXP = "";

    private void Update()
    {
        SF_Username = username;
        SF_UserRank = userRank;
        SF_UserEXP = userEXP;
    }
}