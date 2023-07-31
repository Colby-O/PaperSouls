#ifndef PACKET_H_
#define PACKET_H_

#include <string.h>

enum PacketType
{
    INIT_CONNECTION = 0,
};

#pragma pack(push, 1)

struct Packet
{
    unsigned int packetType;
    unsigned int length;
};

struct TestPacket
{
    Packet header;
    char msg[32];
};

#pragma pack(pop)

#endif