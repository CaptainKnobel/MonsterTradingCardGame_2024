﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Http.Endpoints;

namespace MonsterTradingCardGame_2024.Http
{
    internal class HttpServer
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly int port = 10001;
        private readonly IPAddress ip = IPAddress.Loopback;

        private TcpListener tcpListener;
        public Dictionary<string, IHttpEndpoint> Endpoints { get; private set; } = new Dictionary<string, IHttpEndpoint>();

        public HttpServer() {
            port = 10001;
            ip = IPAddress.Loopback;

            tcpListener = new TcpListener(ip, port);
        }

        public HttpServer(int port)
        {
            this.port = port;
            ip = IPAddress.Loopback;

            tcpListener = new TcpListener(ip, port);
        }
        public HttpServer(IPAddress ip, int port)
        {
            this.port = port;
            this.ip = ip;

            tcpListener = new TcpListener(ip, port);
        }

        public void Run()
        {
            tcpListener.Start();
            Console.WriteLine($"Server is running on {ip}:{port}");
            while (!cts.Token.IsCancellationRequested)
            {
                if (tcpListener.Pending())
                {
                    // ----- 0. Accept the TCP-Client and create the reader and writer -----
                    var clientSocket = tcpListener.AcceptTcpClient();
                    var httpProcessor = new HttpProcessor(this, clientSocket);
                    // Use ThreadPool to make it multi-threaded
                    ThreadPool.QueueUserWorkItem(o => httpProcessor.Process());
                }
                Thread.Sleep(100); // To reduce CPU load
            }
        }

        public void Stop()
        {
            cts.Cancel();
            tcpListener.Stop();
            Console.WriteLine("Server has been stopped.");
        }

        public void RegisterEndpoint(string path, IHttpEndpoint endpoint)
        {
            Endpoints.Add(path, endpoint);
        }
    } // <- End of HttpServer class
} // <- End of MonsterTradingCardGame_2024.Http namesspace

/*
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Our first simple HTTP-Server: http://localhost:10001/");

// ===== I. Start the HTTP-Server =====
var httpServer = new TcpListener(IPAddress.Loopback, 10001);
httpServer.Start();

while (true)
{
    // ----- 0. Accept the TCP-Client and create the reader and writer -----
    var clientSocket = httpServer.AcceptTcpClient();
    using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
    using var reader = new StreamReader(clientSocket.GetStream());

    // ----- 1. Read the HTTP-Request -----
    string? line;

    // 1.1 first line in HTTP contains the method, path and HTTP version
    line = reader.ReadLine();
    if ( line != null )
        Console.WriteLine(line);

    // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
    int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
        if (line == "")
        {
            break;  // empty line indicates the end of the HTTP-headers
        }

        // Parse the header
        var parts = line.Split(':');
        if (parts.Length == 2 && parts[0] == "Content-Length")
        {
            content_length = int.Parse(parts[1].Trim());
        }
    }

    // 1.3 read the body if existing
    if ( content_length>0 )
    {
        var data = new StringBuilder(200);
        char[] chars = new char[1024];
        int bytesReadTotal = 0;
        while ( bytesReadTotal < content_length)
        {
            var bytesRead = reader.Read(chars, 0, chars.Length);
            bytesReadTotal += bytesRead;
            if (bytesRead == 0)
                break;
            data.Append(chars, 0, bytesRead);
        }
        Console.WriteLine( data.ToString() );
    }

    // ----- 2. Do the processing -----
    // .... 

    Console.WriteLine("----------------------------------------");

    // ----- 3. Write the HTTP-Response -----
    var writerAlsoToConsole = new StreamTracer(writer);  // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console

    writerAlsoToConsole.WriteLine("HTTP/1.0 200 OK");    // first line in HTTP-Response contains the HTTP-Version and the status code
    writerAlsoToConsole.WriteLine("Content-Type: text/html; charset=utf-8");     // the HTTP-headers (in HTTP after the first line, until the empy line)
    writerAlsoToConsole.WriteLine();
    writerAlsoToConsole.WriteLine("<html><body><h1>Hello World!</h1></body></html>");    // the HTTP-content (here we just return a minimalistic HTML Hello-World)

    Console.WriteLine("========================================");
}
*/