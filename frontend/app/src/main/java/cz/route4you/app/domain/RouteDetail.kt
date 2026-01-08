package cz.route4you.app.domain

data class RouteDetail(
    val id: String,
    val name: String,
    val grade: String,
    val areaId: String,
    val pathNames: List<String>,
    val pitches: Int,
    val lengthMeters: Int?,
    val style: String,
    val tags: List<String>,
    val ascentsCount: Int,
    val ratingAvg: Double?,
    val ratingsCount: Int
)