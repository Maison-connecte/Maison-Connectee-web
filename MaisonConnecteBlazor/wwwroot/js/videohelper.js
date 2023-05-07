// Crée le blob local pour la vidéo
window.createVideoUrl = async (videoData) => {
    const video = new Uint8Array(videoData)
    const blob = new Blob([video], { type: "video/mp4" })
    const url = URL.createObjectURL(blob)
    return url
}

// Fait jouer la vidéo dans le bon tag video
window.setVideoSource = (videoElementId, url) => {
    const videoElement = document.getElementById(videoElementId)
    videoElement.src = url
}