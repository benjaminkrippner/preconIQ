const fs = require('fs');
const path = require('path');

const envFilePath = path.join(process.cwd(), '.env.local');

function loadExistingEnv(filePath) {
  if (!fs.existsSync(filePath)) {
    return {};
  }

  return fs
    .readFileSync(filePath, 'utf8')
    .split(/\r?\n/)
    .reduce((acc, line) => {
      if (!line || line.trim().startsWith('#') || !line.includes('=')) {
        return acc;
      }

      const [key, ...rest] = line.split('=');
      acc[key.trim()] = rest.join('=').trim();
      return acc;
    }, {});
}

const envMappings = {
  REACT_APP_API_BASE_URL: ['PRECONIQAPIURL'],
  REACT_APP_MSAL_CLIENT_ID: ['MSAL_CLIENT_ID', 'AZURE_CLIENT_ID'],
  REACT_APP_MSAL_API_CLIENT_ID: ['MSAL_API_CLIENT_ID'],
  REACT_APP_MSAL_TENANT_ID: ['MSAL_TENANT_ID', 'AZURE_TENANT_ID'],
  REACT_APP_MSAL_REDIRECT_URL: ['MSAL_REDIRECT_URL', 'SWAGGER_REDIRECT_URL'],
};

const existingEnv = loadExistingEnv(envFilePath);
const generatedLines = [];

Object.entries(envMappings).forEach(([targetKey, sourceKeys]) => {
  if (process.env[targetKey] || existingEnv[targetKey]) {
    return;
  }

  const sourceValue = sourceKeys.map((sourceKey) => process.env[sourceKey]).find(Boolean);

  if (sourceValue) {
    generatedLines.push(`${targetKey}=${sourceValue}`);
  }
});

if (generatedLines.length === 0) {
  console.log('prepare-env: no environment variables to inject.');
  process.exit(0);
}

const header = fs.existsSync(envFilePath)
  ? '\n# Auto-generated from deployment environment variables\n'
  : '# Auto-generated from deployment environment variables\n';

fs.appendFileSync(envFilePath, `${header}${generatedLines.join('\n')}\n`);
console.log(`prepare-env: wrote ${generatedLines.length} value(s) to .env.local`);
