using System.DirectoryServices;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

// C#12 Feature: Primary constructor
public class HomeController : Controller
{
    [HttpGet($"xpath-injection/{{user}}")]
    public string? XPathInjection(string user)
    {
        XmlDocument doc = new();
        doc.Load("bookstore.xml");
        XmlNode root = doc.DocumentElement;
        XmlNamespaceManager nsmgr = new(doc.NameTable);
        nsmgr.AddNamespace("bk", "urn:newbooks-schema");

        var node = root?.SelectSingleNode(
            "descendant::bk:book[bk:author/bk:last-name='"+user+"']", nsmgr);

        return node?.InnerText;
    }
    
    public async void SimpleXss1(string userInfo)
    {
        var context = ControllerContext.HttpContext;
        // High: XSS
        await context.Response.WriteAsync("<body>"+ userInfo +"</body>");
    }
    
    public async void SimpleXss2(string userInfo)
    {
        // High: XSS - NOT DETECTED
        await HttpContext.Response.WriteAsync("<body>"+ userInfo +"</body>");
    }
    
    [HttpGet("xxe/{xmlString}")]
    public void DoXxe(String xmlString)
    {
        var xmlDoc = new XmlDocument();
        // High: XXE
        xmlDoc.LoadXml(xmlString);
    }
    
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [HttpGet($"ldap-injection/{{user}}")]
    public void LdapInjection(string user)
    {
        DirectoryEntry directoryEntry = new("LDAP://DC=mycompany,DC=com");
        var directorySearcher = new DirectorySearcher(directoryEntry) // Doesn't pick up the vulnerability if target-typed news are used
        {
            // High: LDAP injection
            Filter = "(&(objectClass=user)(|(cn=" + user + ")(sAMAccountName=" + user + ")))"
        };

        SearchResult result = directorySearcher.FindOne();
    }
}