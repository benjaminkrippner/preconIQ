import azure.functions as func

# Single Function App instance shared by all routes
app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)
