#include <socket.h>

int Socket::init()
{
    // Creates a socket
    if ((this->fd = socket(AF_INET, SOCK_STREAM, 0)) == 0)
    {
        std::cerr << "Cannot create a socket!\n";
        return -1;
    }

    // Allows you reuse socket
    if (setsockopt(this->fd, SOL_SOCKET, SO_REUSEADDR, (char*)&(this->opt), sizeof(this->opt))) 
    {
        std::cerr << "The socket argument is not a valid file descriptor!\n";
        return -1;
    }

    this->hint.sin_family = AF_INET;
    this->hint.sin_addr.s_addr = INADDR_ANY;
    this->hint.sin_port = htons(this->port);
    inet_pton(AF_INET, "0.0.0.0", &this->hint.sin_addr);

    if (bind(this->fd, (struct sockaddr*)&this->hint, sizeof(this->hint)) < 0) 
    {
        std::cerr << "Cannot bind to IP/port!\n";
        return -1;
    }

    if (::listen(this->fd, SOMAXCONN) < 0)
    {
        std::cerr << "Cannot listen!\n";
        return -1;
    }

    int flags = fcntl(this->fd, F_GETFL);
    fcntl(this->fd, F_SETFL, flags | O_NONBLOCK);

    return 0;
}

int Socket::listen()
{
    return accept(this->fd, reinterpret_cast<sockaddr*>(&hint), &serverLength);
}

Packet* Socket::recieve(int fd)
{
    std::vector<std::byte> data;

    std::byte buffer[512];
    size_t bytesToRead = sizeof(Packet);
    while (bytesToRead > 0)
    {
        int length = recv(fd, buffer, (sizeof(buffer) < bytesToRead) ? sizeof(buffer) : bytesToRead, 0);
        if (length <= 0)
            return nullptr;

        bytesToRead -= length;
        data.insert(data.end(), buffer, buffer + length);
    }

    Packet header = *reinterpret_cast<Packet*>(data.data());

    bytesToRead = header.length;

    while (bytesToRead > 0)
    {
        int length = recv(fd, buffer, (sizeof(buffer) < bytesToRead) ? sizeof(buffer) : bytesToRead, 0);
        if (length <= 0)
            return nullptr;

        bytesToRead -= length;
        data.insert(data.end(), buffer, buffer + length);
    }

    std::byte* packet = new std::byte[data.size()];

    memcpy(packet, data.data(), data.size());

    return reinterpret_cast<Packet*>(packet);
}

int Socket::send(int client, std::string msg) 
{
    return ::send(client, msg.c_str(), strlen(msg.c_str()), 0);
}
