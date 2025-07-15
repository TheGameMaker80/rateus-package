#import <StoreKit/StoreKit.h>

extern "C" {
    void RequestReview()
    {
        if(@available(iOS 10.3, *))
        {
            [SKStoreReviewController requestReview];
        }
    }
}
