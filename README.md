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