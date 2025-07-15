#import <StoreKit/StoreKit.h>

extern "C" {
    void _RequestReview()
    {
        if(@available(iOS 10.3, *))
        {
            [SKStoreReviewController requestReview];
        }
    }
}
