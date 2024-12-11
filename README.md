# ZonefyDotnet

Here’s a summary of how to install PostgreSQL 17 on Ubuntu 24.04:

Overview: PostgreSQL 17 offers new features like improved monitoring, performance, logical replication, and security. This guide covers installation, basic configuration, and remote connection setup.

Prerequisites:

Ubuntu 24.04
Root or sudo access
Installation Steps:

# Add PostgreSQL Repository:
# Update the package list:
	- sudo apt update
# Add PostgreSQL 17 repository:
	- sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
# Import the repository signing key:
	- curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo gpg --dearmor -o /etc/apt/trusted.gpg.d/postgresql.gpg
# Update the package list again:
	- sudo apt update
# Install PostgreSQL 17:
# Install PostgreSQL:
	- sudo apt install postgresql-17
# Start and enable PostgreSQL service:
	- sudo systemctl start postgresql
	- sudo systemctl enable postgresql
# Verify installation:
	- psql --version
# Configure PostgreSQL:
# Enable remote connections by editing postgresql.conf:
	- sudo nano /etc/postgresql/17/main/postgresql.conf
	set listen_addresses = '*'
# Use md5 or scram-sha-256 password authentication by editing pg_hba.conf:
	- sudo sed -i '/^host/s/ident/md5/' /etc/postgresql/17/main/pg_hba.conf 
						# or
	- sudo sed -i '/^host/s/ident/scram-sha-256/' /etc/postgresql/17/main/pg_hba.conf
# Allow connections from all hosts:
	- echo "host all all 0.0.0.0/0 md5" | sudo tee -a /etc/postgresql/17/main/pg_hba.conf
	or
	- echo "host all all 0.0.0.0/0 scram-sha-256" | sudo tee -a /etc/postgresql/17/main/pg_hba.conf
# Restart PostgreSQL:
	- sudo systemctl restart postgresql
# Allow PostgreSQL port through the firewall:
	- sudo ufw allow 5432/tcp
# Connect to PostgreSQL:
# Connect as postgres user:
	- sudo -u postgres psql
# Set a password for the postgres user:
	- ALTER USER postgres PASSWORD 'your_password';
# Conclusion: PostgreSQL 17 is successfully installed, configured for remote access, and ready for use.


# Installing Redis on Ubuntu machine

# Step 1: Update System Packages
	Update the package list to ensure your system is up to date:
		
		- sudo apt update && sudo apt upgrade -y

# Step 2: Install Redis
	Install Redis from the default Ubuntu repositories:

		- sudo apt install redis -y

# Step 3: Start and Enable Redis Service
	Start the Redis service and enable it to start at boot:

		- sudo systemctl start redis
		- sudo systemctl enable redis

# Step 4: Verify Redis Installation
	Check the Redis version to confirm installation:

		- redis-cli --version

# Test Redis by running the CLI:

		- redis-cli ping
	You should see:

		- PONG

# Step 5: Configure Redis
	Edit the Redis configuration file for remote access or custom settings:

		- sudo nano /etc/redis/redis.conf

	To allow remote connections: Find the bind 127.0.0.1 line and change it to:
		
		- bind 0.0.0.0

	To secure remote connections: Enable a password by adding or uncommenting: preese ctrl+W to search within the file
		
		- requirepass YourStrongPassword

# Restart Redis to apply changes:

		- sudo systemctl restart redis

# Step 6: Open Redis Port (Optional)
	Allow Redis port (default: 6379) through the firewall if remote access is needed:

		- sudo ufw allow 6379

# Step 7: Test Redis Connection
	Local connection:
		- redis-cli
	Run:
		- AUTH YourStrongPassword
		- ping
	You should see:
		- PONG

# Remote connection (from another machine with Redis CLI installed):
		
		- redis-cli -h your_server_ip -p 6379 -a YourStrongPassword

# Step 8: Secure Redis (Optional)
	For production environments:

	Use a firewall to restrict access to the Redis port.
	Use strong passwords.
	Configure Redis to listen only to trusted IP addresses.

Redis is now installed and ready to use on your Ubuntu VPS!