import type { Component } from 'solid-js';
import { HopeThemeConfig, HopeProvider, NotificationsProvider } from '@hope-ui/solid'
import { Layout } from './components/Layout';
import { Router } from '@solidjs/router';
import { Auth0 } from '@afroze9/solid-auth0';

const config: HopeThemeConfig = {
  lightTheme: {
  }
}

const App: Component = () => {
  return (
    <Auth0
      domain='teamly.us.auth0.com'
      clientId='InPTCm0pFBIovGrvm0I3qJGs5XnVgJV5'
      audience='projectmanagement'
      logoutRedirectUri={`${window.location.origin}/`}
      loginRedirectUri={`${window.location.origin}/`}
      scope='openid profile email read:project write:project update:project delete:project read:company write:company update:company delete:company'
    >
      <Router>
        <HopeProvider config={config}>
          <NotificationsProvider>
            <Layout />
          </NotificationsProvider>
        </HopeProvider>
      </Router>
    </Auth0>
  );
};

export default App;
