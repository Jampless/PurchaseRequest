window.addEventListener('message', (event) => {
    if (event.origin !== 'http://192.168.0.7:8484') return;
    if (event.data.username) {
        sessionStorage.setItem('username', event.data.username);
    }
    if (event.data.session_id) {
        sessionStorage.setItem('session_id', event.data.session_id);
    }
    if (event.data.position) {
        sessionStorage.setItem('position', event.data.position);
    }
    if (event.data.company) {
        sessionStorage.setItem('company', event.data.company);
    }
    if (event.data.email) {
        sessionStorage.setItem('email', event.data.email);
    }
});