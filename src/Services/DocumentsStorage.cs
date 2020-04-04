using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.EntityFrameworkCore;
using Norka.Models;

namespace Norka.Services
{
    public class DocumentsStorage
    {
        private static readonly Object s_lock = new Object();
        private static DocumentsStorage instance = null;

        public LiteDatabase db;
        public event EventHandler DocumentAdded = delegate { };
        public event EventHandler DocumentRemoved = delegate { };


        public DocumentsStorage()
        {
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appConfigPath = Path.Combine(configPath, "Norka");
            EnsureFolderCreated(appConfigPath);

            db = new LiteDatabase(Path.Combine(appConfigPath, "documents.db"));
        }
        public static DocumentsStorage Instance
        {
            get
            {
                if (instance != null) return instance;
                Monitor.Enter(s_lock);
                DocumentsStorage temp = new DocumentsStorage();
                Interlocked.Exchange(ref instance, temp);
                Monitor.Exit(s_lock);
                return instance;
            }
        }

        void EnsureFolderCreated(string appConfigPath)
        {
            if (!Directory.Exists(appConfigPath))
            {
                Directory.CreateDirectory(appConfigPath);
            }
        }

        public IEnumerable<Document> All()
        {
            var docs = db.GetCollection<Document>().FindAll();
            return docs;
        }

        public Document AddDocument()
        {
            var doc = new Document() { Title = "New document" };
            db.GetCollection<Document>().Insert(doc);

            DocumentAdded(this, EventArgs.Empty);
            return doc;
        }

        public void RemoveDocument(Document doc)
        {
            db.GetCollection<Document>().Delete(doc.Id);
            DocumentRemoved(this, EventArgs.Empty);
        }
    }
}