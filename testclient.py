import socket
import time
import random

def tcp_client():
    host = '127.0.0.1'  # Server's hostname or IP address
    port = 50000         # Server's port number
    max_retries = 5     # Maximum number of reconnection attempts
    base_delay = 1      # Initial delay between attempts in seconds    
    message = 'Hello, Server!'  # Message to send


    for attempt in range(1, max_retries + 1):
        client_socket = None
        try:
            client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  # Create a TCP/IP socket
            client_socket.connect((host, port))  # Connect to the server
            client_socket.sendall(message.encode())  # Send data
            print('Sending to server:', message)  # Display server response
            response = client_socket.recv(1024).decode()  # Receive response
            print('Received from server:', response)  # Display server response
            break
        except (socket.error, socket.timeout) as e:
            print(f'Connection attempt {attempt} failed: {e}')
            if attempt == max_retries:
                print('Max retries reached. Exiting.')
                return
            # Exponential backoff with jitter
            delay = base_delay * (2 ** (attempt - 1))
            delay += random.uniform(0, 0.1 * delay)  # Add jitter
            print(f'Retrying in {delay:.2f} seconds...')
            time.sleep(delay)
        finally:
            try:
                client_socket.close()
            except NameError:
                pass  # client_socket was never created

if __name__ == '__main__':
    tcp_client()
