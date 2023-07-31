CC = g++
CFLAGS = -O3 -Wall -Wno-unused-result -std=c++17 -I ./include

TARGET = server
SRC_DIR = src
OBJ_DIR = build

CSRCS = $(shell find ./src/  -type f -name '*.cpp' -printf "%f\n")
OBJS = $(addprefix ${OBJ_DIR}/, $(CSRCS:.cpp=.o))

all: ${TARGET} clean

${TARGET}: ${OBJ_DIR} ${OBJS}
	$(CC) -o ${TARGET} ${OBJS} ${CFLAGS}

${OBJ_DIR}/%.o: ${SRC_DIR}/%.cpp
	$(CC) -c $< -o $@ ${CFLAGS}

${OBJ_DIR}:
	mkdir -p ${OBJ_DIR}

clean:
	rm -rf ${OBJ_DIR}