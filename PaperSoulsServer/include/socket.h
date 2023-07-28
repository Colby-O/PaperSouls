#ifndef SOCKET_H_
#define SOCKET_H_

#include <sys/socket.h>
#include <arpa/inet.h>
#include <iostream>
#include <fcntl.h>
#include <string>
#include <string_view>
#include <error.h>
#include <vector>
#include <algorithm>
#include "packet.h"

class Socket
{
private:
    unsigned int port;
    int fd;
    socklen_t serverLength;
    int opt = 1;
    sockaddr_in hint;

public:
    Socket(unsigned int port) : port(port), serverLength(sizeof(sockaddr_in)) {}

    int init();
    int listen();
    Packet* recieve(int fd);
    int send(int client, std::string msg);
};

#endif