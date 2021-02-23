using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Diagnostics;

namespace HTTP5203_Lab5_DanielGuinto.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            //Create a list of book models
            IList<Models.Book> bookList = new List<Models.Book>();

            //Sets up loading books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            //Checks if xml file exists, will load if it does exists. Doesn't load if file doesn't exist
            if (System.IO.File.Exists(path))
            {
                //Loads document
                doc.Load(path);

                //Creates nodelist of elements (nodes) in xml file
                XmlNodeList books = doc.GetElementsByTagName("book");

                //Loops through each node to get values
                foreach (XmlElement b in books)
                {
                    Models.Book book = new Models.Book();

                    book.id = Int32.Parse(b.GetElementsByTagName("id")[0].InnerText);
                    book.bookTitle = b.GetElementsByTagName("title")[0].InnerText;
                    //book.authorTitle = b.GetAttribute("title");
                    book.authorFname = b.GetElementsByTagName("firstname")[0].InnerText;
                    var checkAuthorMname = b.GetElementsByTagName("middlename")[0];
                    if (checkAuthorMname == null)
                    {
                        book.authorMname = "";
                    }
                    else
                    {
                        book.authorMname = checkAuthorMname.InnerText;
                    }
                    book.authorLname = b.GetElementsByTagName("lastname")[0].InnerText;

                    //Adds values to bookList
                    bookList.Add(book);
                }

            }

            //Returns bookList for View
            return View(bookList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();
            return View(book);
        }
        public IActionResult Create(Models.Book b)
        {
            //Sets up loading books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            //Checks if xml file exists
            if (System.IO.File.Exists(path))
            {
                //Loads file if it exists and creates new book
                doc.Load(path);

                //Create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //Get root element and append book node to it
                doc.DocumentElement.AppendChild(book);

            }
            else
            {
                //If file doesn't exist, creates the file and adds a new book
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("books");

                //Create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //Get root element and append book node to it
                doc.AppendChild(book);
                doc.AppendChild(root);


            }
            //Saves file to specific "path"
            doc.Save(path);

            return RedirectToAction("Index");
        }

        private XmlElement _CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
            //Creates nodes with values that will be added to the XML file
            XmlElement book = doc.CreateElement("book");

            //Checks the last id value in the XML file, and increases the next id value by 1
            XmlNode id = doc.CreateElement("id");
            var lastId = Int32.Parse(doc.SelectSingleNode("//book[last()]/id").InnerText);
            var nextId = lastId + 1;
            var fill = "";
            if (nextId < 10)
            {
                fill = "000";
            }
            else if (nextId >= 10 && nextId < 100)
            {
                fill = "00";
            }
            else if (nextId >= 100 && nextId <= 1000)
            {
                fill = "0";
            }
            else
            {
                fill = "";
            }
            id.InnerText = (fill + nextId).ToString();

            XmlNode bookTitle = doc.CreateElement("title");
            bookTitle.InnerText = newBook.bookTitle;

            XmlNode author = doc.CreateElement("author");
            XmlAttribute title = doc.CreateAttribute("title");
            title.Value = newBook.authorTitle;
            author.Attributes.Append(title);
            XmlNode firstName = doc.CreateElement("firstname");
            firstName.InnerText = newBook.authorFname;
            XmlNode middleName = doc.CreateElement("middlename");
            middleName.InnerText = newBook.authorMname;
            XmlNode lastName = doc.CreateElement("lastname");
            lastName.InnerText = newBook.authorLname;

            //Appends child nodes to parent nodes
            author.AppendChild(firstName);
            author.AppendChild(middleName);
            author.AppendChild(lastName);

            book.AppendChild(id);
            book.AppendChild(bookTitle);
            book.AppendChild(author);

            //Checks number of book nodes in xml file
            int nodeCount = doc.SelectNodes("//book").Count;
            //Gets the first book node in XML file
            XmlNode firstNode = doc.SelectSingleNode("//book[1]");

            //Debug lines to check values
            Debug.WriteLine(nodeCount);
            Debug.WriteLine(firstNode);
            
            //Deletes first book node if nodes past 5 books
            if (nodeCount > 4)
            {
                XmlNode parentNode = firstNode.ParentNode;
                parentNode.RemoveChild(firstNode);
                Debug.WriteLine("Delete Node Now");
            }

            return book;
        }
    }
}
