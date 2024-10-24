package com.laboratory3;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.Scanner;

public class Client {
    public static void main(String[] args) {
        try {
            Socket socket = new Socket("localhost", 10010);
            System.out.println("=========================================================");
            System.out.println("Connected to server at " + socket.getRemoteSocketAddress());

            ObjectOutputStream outputStream = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream inputStream = new ObjectInputStream(socket.getInputStream());

            System.out.println(">> SERVER: [" + inputStream.readObject() + "]");
            Scanner scanner = new Scanner(System.in);
            System.out.print("Enter the number of messages to send: ");
            int n = scanner.nextInt();
            scanner.nextLine();
            outputStream.writeObject(n);

            System.out.println(">> SERVER: [" + inputStream.readObject() + "]");
            for (int i = 0; i < n; i++) {
                System.out.print("Enter message " + (i + 1) + ": ");
                String content = scanner.nextLine();
                outputStream.writeObject(new Message(i, content));
            }

            inputStream.close();
            outputStream.close();
            socket.close();
            System.out.println("Closing connection to server at " + socket.getRemoteSocketAddress());
            System.out.println("=========================================================");
        } catch (IOException | ClassNotFoundException e) {
            e.printStackTrace();
        }
    }
}
