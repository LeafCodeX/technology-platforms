package com.laboratory3;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

public class Server {
    public static void main(String[] args) {
        try {
            ServerSocket serverSocket = new ServerSocket(10010);
            System.out.println("=========================================================");
            System.out.println("Server started on localhost:10010. Connecting...");

            while (true) {
                Socket socket = serverSocket.accept();
                System.out.println("Connected to client at " + socket.getRemoteSocketAddress());
                new ServerThread(socket).start();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
