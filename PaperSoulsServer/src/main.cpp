#include <iostream>
#include <cstdlib>
#include <string>
#include <unistd.h>
#include "socket.h"

int main()
{
    std::cout << "Starting Paper Souls Server..." << std::endl;

    Socket* socket = new Socket(8080);

    socket->init();

    while (true)
    {
        int client = socket->listen();
        if (client > 0) {
            std::string m = "Yo!";
            sleep(1);
            std::cout << "Sending " << m << " to " << client << std::endl;
            socket->send(client, m);
        }
        TestPacket* msg = (TestPacket*)socket->recieve(client);
        if (msg != nullptr)
        {
            std::cout << msg->msg << std::endl;
        }
        sleep(1);
    }

    return EXIT_SUCCESS;
}