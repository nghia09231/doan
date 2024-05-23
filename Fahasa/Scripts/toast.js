function toast(toast) {

    let color = {
        success: '#198754',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#0d6efd',
    }

    Toastify({
        text: "",
        duration: 3000,
        close: true,
        gravity: "bottom",
        position: "left",
        stopOnFocus: true,
        style: {
            background: toast?.type ? color[toast.type] : color.info,
            fontSize: '16px'
        },
        ...toast
    }).showToast();
}