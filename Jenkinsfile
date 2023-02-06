pipeline {
    agent any

    stages {
        stage('Clone') {
            steps {
                git url: 'https://github.com/foxminchan/SaoVietAPI.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet restore'
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test --logger "trx;LogFileName=test_results.trx"'
            }
        }
    }
}