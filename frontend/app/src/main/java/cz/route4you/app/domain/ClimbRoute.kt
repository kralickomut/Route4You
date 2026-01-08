package cz.route4you.app.domain


data class ClimbRoute(
    val id: String,
    val name: String,
    val grade: String,
    val areaId: String,
    val ascentsCount: Int,
    val ratingAvg: Double?,
    val ratingsCount: Int
)