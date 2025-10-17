import React from 'react'
import ReactDOM from 'react-dom/client'
import { CUSTOM_CSS_URL, FONT_AWESOME_URL, SHIFT_CSS_URL, SIMPLEBAR_CSS_URL, SIMPLEBAR_JS_URL } from './helpers/constants';
import App from './App'
import './main.css'

(document.getElementById("shift_stylesheet") as HTMLLinkElement).href = SHIFT_CSS_URL;
(document.getElementById("font_awesome_stylesheet") as HTMLLinkElement).href = FONT_AWESOME_URL;
(document.getElementById("simplebar_stylesheet") as HTMLLinkElement).href = SIMPLEBAR_CSS_URL;
(document.getElementById("simplebar_script") as HTMLScriptElement).src = SIMPLEBAR_JS_URL;
(document.getElementById("custom_stylesheet") as HTMLLinkElement).href = CUSTOM_CSS_URL;

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
)
