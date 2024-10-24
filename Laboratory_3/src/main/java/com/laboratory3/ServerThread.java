package com.laboratory3;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;

public class ServerThread extends Thread {
    private Socket socket;

    public ServerThread(Socket socket) {
        this.socket = socket;
    }

    public void run() {
        try {
            ObjectOutputStream outputStream = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream inputStream = new ObjectInputStream(socket.getInputStream());

            outputStream.writeObject("READY");
            outputStream.writeObject("WAITING_FOR_MESSAGES");
            int n = (int) inputStream.readObject();
            System.out.println("Received number of messages: " + n);

            for (int i = 0; i < n; i++) {
                Message message = (Message) inputStream.readObject();
                System.out.println("Received message: " + message);
            }

            System.out.println("Connection to client at " + socket.getRemoteSocketAddress() + " closed");
            System.out.println("=========================================================");


        } catch (IOException | ClassNotFoundException e) {
            System.out.println("Connection to client at " + socket.getRemoteSocketAddress() + " closed");
            System.out.println("=========================================================");
        } finally {
            try {
                socket.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
