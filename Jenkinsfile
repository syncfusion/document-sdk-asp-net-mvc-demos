#!groovy
properties([
    pipelineTriggers([
        cron('0 17 * * 4'), 
        cron('0 9 * * 2')  
    ])
])
node(isProtectedBranch() ? 'AspComponent' : 'AspComponents') {
    timeout(time:90){
        try {
            deleteDir();
            stage('Import') {
                git url: 'https://gitea.syncfusion.com/essential-studio/ej2-groovy-scripts.git', branch: 'master', credentialsId: env.GiteaCredentialID;
                shared = load 'src/shared.groovy'; 
            }

            stage('Checkout') {
                checkout scm;
            }

            stage('Workflow Validation') {
                shared.checkBranchName();
                shared.checkCommitMessage();
                shared.validateMRDescription();
            }

            stage('Install') {
                runShell('npm -v');
                runShell('npm install');
                runShell('git config --global user.email "essentialjs2@syncfusion.com"');
                runShell('git config --global user.name "essentialjs2"');
                runShell('git config --global core.longpaths true');
            }

            stage('Code Leaks Analysis'){
                try{
                    shared.initGitleaks();
                    runShell('npm run gitleaks-test')
                }
                finally{
                    if(fileExists('GitLeaksReport.json'))
                    {
                        archiveArtifacts artifacts:'GitLeaksReport.json';
                    }
                }               
            }

            stage('Build') {
                runShell('gulp update-nuget-config');
                if(isProtectedBranch()){
                    runShell('npm run update-service-urls');
                }
                runShell('npm run build');
                runShell('gulp mvc-version-update');
                runShell('gulp aspmvc-build --option ./EJ2MVCSampleBrowser.csproj');
                shared.artifactFiles();
            }

            // stage('Deploy Build Samples') {
            //     if(isProtectedBranch()){
            //         runShell('npm run deploy-build-samples');
            //     }
            // }

            stage('Publish') {
                if(isProtectedBranch()) {
                    runShell('gulp sitemap-generate --option local-sitemap');
                    runShell('npm run publish'); 
                }
            }
            deleteDir();
        }
        catch(Exception e) {
            if(isProtectedBranch()) {
                shared.runShell('gulp publish-report --platform aspnetmvc');
            }
            shared.throwError(e);
            deleteDir();
        }
    }
}

def runShell(String command) {
    if(isUnix()) {
        sh command;
    }
    else {
        bat command;
    }
}

def isProtectedBranch() {
    return env.githubSourceBranch == env.STAGING_BRANCH || String.valueOf(env.githubSourceBranch).startsWith('hotfix/') || String.valueOf(env.githubSourceBranch).startsWith('release/');
}
