// Crée le blob local pour la vidéo
window.creeURLVideo = async (donneesVideo) => {
    const video = new Uint8Array(donneesVideo)
    const blob = new Blob([video], { type: "video/mp4" })
    const url = URL.createObjectURL(blob)
    return url
}

// Fait jouer la vidéo dans le bon tag video
window.definirSourceVideo = (IDElementVideo, url) => {
    const elementVideo = document.getElementById(IDElementVideo)
    elementVideo.src = url
}