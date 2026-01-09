import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import { cssHelper } from "./helpers/cssHelper";
import './main.css'

cssHelper.setMainCssFiles();

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
)
